using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Reflection;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Registration
{
    public interface IController {}
    public interface IAuthentication {}
    public interface IFormsAuthenticationService {}
    public interface IMembershipService {}

    public class FormsAuthenticationServiceImpl : IFormsAuthenticationService {}
    public class MembershipServiceImpl : IMembershipService {}
    public class SpecificMembershipServiceImpl : IMembershipService {}
    public class HttpDigestAuthentication : IAuthentication {}

    public class AmbiguousConstructors
    {
        public AmbiguousConstructors(string first, int second) { StringArg = first; IntArg = second; }
        public AmbiguousConstructors(int first, string second) { IntArg = first; StringArg = second; }

        public int IntArg { get; set ; }
        public string StringArg { get; set ; }
    }

    public class AmbiguousConstructorsWithAttribute
    {
        [ImportingConstructorAttribute]
        public AmbiguousConstructorsWithAttribute(string first, int second) { StringArg = first; IntArg = second; }
        public AmbiguousConstructorsWithAttribute(int first, string second) { IntArg = first; StringArg = second; }

        public int IntArg { get; set ; }
        public string StringArg { get; set ; }
    }

    public class LongestConstructorWithAttribute
    {
        [ImportingConstructorAttribute]
        public LongestConstructorWithAttribute(string first, int second)  { StringArg = first; IntArg = second; }
        public LongestConstructorWithAttribute(int first)  { IntArg = first; }

        public int IntArg { get; set ; }
        public string StringArg { get; set ; }
    }

    public class LongestConstructorShortestWithAttribute
    {
        public LongestConstructorShortestWithAttribute(string first, int second)  { StringArg = first; IntArg = second; }
        [ImportingConstructorAttribute]
        public LongestConstructorShortestWithAttribute(int first)  { IntArg = first; }

        public int IntArg { get; set ; }
        public string StringArg { get; set ; }
    }


    public class ConstructorArgs
    {
        public ConstructorArgs()
        {
            IntArg = 10;
            StringArg = "Hello, World";
        }

        public int IntArg { get; set ; }
        public string StringArg { get; set ; }
    }

    public class AccountController : IController
    {
        public IMembershipService MembershipService { get; private set; }

        public AccountController(IMembershipService membershipService)
        {
            this.MembershipService = membershipService;
        }

        public AccountController(IAuthentication auth)
        {
        }
    }

    public class HttpRequestValidator
    {
        public IAuthentication authenticator;
        
        public HttpRequestValidator(IAuthentication authenticator) {}
    }

    public class ManyConstructorsController : IController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        public HttpRequestValidator Validator { get; set; }

        public ManyConstructorsController() {}
        public ManyConstructorsController(
            IFormsAuthenticationService formsService) 
        {
            FormsService = formsService;
        }

        public ManyConstructorsController(
            IFormsAuthenticationService formsService, 
            IMembershipService membershipService) 
        {
            FormsService = formsService;
            MembershipService = MembershipService;
        }

        public ManyConstructorsController(
            IFormsAuthenticationService formsService, 
            IMembershipService membershipService, 
            HttpRequestValidator validator)
        {
            FormsService = formsService;
            MembershipService = membershipService;
            Validator = validator;
        }
    }


    [TestClass]
    public class PartBuilderUnitTests
    {
        [TestMethod]
        public void ManyConstructorsControllerFindLongestConstructor_ShouldSucceed()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<FormsAuthenticationServiceImpl>().Export<IFormsAuthenticationService>();
            ctx.ForType<HttpDigestAuthentication>().Export<IAuthentication>();
            ctx.ForType<MembershipServiceImpl>().Export<IMembershipService>();
            ctx.ForType<HttpRequestValidator>().Export();
            ctx.ForType<ManyConstructorsController>().Export();

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(FormsAuthenticationServiceImpl), 
                typeof(HttpDigestAuthentication), 
                typeof(MembershipServiceImpl), 
                typeof(HttpRequestValidator), 
                typeof(ManyConstructorsController)), ctx); 

            Assert.IsTrue(catalog.Parts.Count() == 5);

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var item = container.GetExportedValue<ManyConstructorsController>();

            Assert.IsTrue(item.Validator != null);
            Assert.IsTrue(item.FormsService != null);
            Assert.IsTrue(item.MembershipService != null);
        }

        [TestMethod]
        public void ManyConstructorsControllerFindLongestConstructorAndImportByName_ShouldSucceed()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<FormsAuthenticationServiceImpl>().Export<IFormsAuthenticationService>();
            ctx.ForType<HttpDigestAuthentication>().Export<IAuthentication>();
            ctx.ForType<MembershipServiceImpl>().Export<IMembershipService>();
            ctx.ForType<SpecificMembershipServiceImpl>().Export<IMembershipService>( (c) => c.AsContractName("membershipService") );
            ctx.ForType<HttpRequestValidator>().Export();
            ctx.ForType<ManyConstructorsController>().SelectConstructor( null, (pi, import) => 
            { 
                if(typeof(IMembershipService).IsAssignableFrom(pi.ParameterType))
                    import.AsContractName("membershipService");
            }).Export();

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(FormsAuthenticationServiceImpl), 
                typeof(HttpDigestAuthentication), 
                typeof(MembershipServiceImpl), 
                typeof(SpecificMembershipServiceImpl), 
                typeof(HttpRequestValidator), 
                typeof(ManyConstructorsController)), ctx); 

            Assert.IsTrue(catalog.Parts.Count() == 6);

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var item = container.GetExportedValue<ManyConstructorsController>();

            Assert.IsTrue(item.Validator != null);
            Assert.IsTrue(item.FormsService != null);
            Assert.IsTrue(item.MembershipService != null);
            Assert.IsTrue(item.MembershipService.GetType() == typeof(SpecificMembershipServiceImpl));
        }

        [TestMethod]
        public void LongestConstructorWithAttribute_ShouldSucceed()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<LongestConstructorWithAttribute>().Export();
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "IntArg" );
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "StringArg" );

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(LongestConstructorWithAttribute),
                typeof(ConstructorArgs)),
                ctx);
            Assert.AreEqual(2, catalog.Parts.Count());
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var item = container.GetExportedValue<LongestConstructorWithAttribute>();
            Assert.AreEqual(10, item.IntArg);
            Assert.AreEqual("Hello, World", item.StringArg);
        }

        [TestMethod]
        public void LongestConstructorShortestWithAttribute_ShouldSucceed()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<LongestConstructorShortestWithAttribute>().Export();
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "IntArg" );
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "StringArg" );

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(LongestConstructorShortestWithAttribute),
                typeof(ConstructorArgs)),
                ctx);
            Assert.AreEqual(2, catalog.Parts.Count());
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var item = container.GetExportedValue<LongestConstructorShortestWithAttribute>();
            Assert.AreEqual(10, item.IntArg);
            Assert.AreEqual(null, item.StringArg);
        }

        [TestMethod]
        public void AmbiguousConstructorWithAttributeAppliedToOne_ShouldSucceed()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<AmbiguousConstructorsWithAttribute>().Export();
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "IntArg" );
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "StringArg" );

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(AmbiguousConstructorsWithAttribute),
                typeof(ConstructorArgs)),
                ctx);
            Assert.AreEqual(2, catalog.Parts.Count());
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var item = container.GetExportedValue<AmbiguousConstructorsWithAttribute>();

            Assert.AreEqual(10, item.IntArg);
            Assert.AreEqual("Hello, World", item.StringArg);
        }


        [TestMethod]
        public void AmbiguousConstructor_ShouldFail()
        {
            var ctx = new RegistrationBuilder();

            ctx.ForType<AmbiguousConstructors>().Export();
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "IntArg" );
            ctx.ForType<ConstructorArgs>().ExportProperties( (m) => m.Name == "StringArg" );

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(AmbiguousConstructors),
                typeof(ConstructorArgs)),
                ctx);
            Assert.AreEqual(catalog.Parts.Count(), 2);
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            ExceptionAssert.Throws<CompositionException>(() =>
            {
                var item = container.GetExportedValue<AmbiguousConstructors>();
            });
        }
    }
}
