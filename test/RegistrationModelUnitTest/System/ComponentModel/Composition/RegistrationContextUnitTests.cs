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
    public interface IFoo {}
    public class FooImplementation1 : IFoo {}
    public class FooImplementation2 : IFoo {}

    [TestClass]
    public class RegistrationBuilderUnitTests
    {
        [TestMethod]
        public void UndecoratedContext_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }

        [TestMethod]
        public void ImplementsIFooNoExport_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesDerivedFrom<IFoo>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }

        [TestMethod]
        public void ImplementsIFooWithExport_ShouldFindTwoParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesDerivedFrom<IFoo>().Export();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 2);
        }

        [TestMethod]
        public void OfTypeInterfaceNoExport_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<IFoo>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }


        [TestMethod]
        public void OfTypeInterfaceWithExport_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<IFoo>().Export();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }

        [TestMethod]
        public void OfTypeOnePart_ShouldFindOnePart()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<FooImplementation1>().Export();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 1);
        }

        [TestMethod]
        public void OfTypeTwoPart_ShouldFindTwoParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<FooImplementation1>().Export();
            ctx.ForType<FooImplementation2>().Export();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 2);
        }

        [TestMethod]
        public void OfTypeTwoPart_ConfigurationAfterConstruction_ShouldFindTwoParts()
        {
            var ctx = new RegistrationBuilder();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            ctx.ForType<FooImplementation1>().Export();
            ctx.ForType<FooImplementation2>().Export();
            Assert.IsTrue(catalog.Parts.Count() == 2);
        }

        [TestMethod]
        public void OfTypeTwoPart_ConfigurationAfterParts_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
            ctx.ForType<FooImplementation1>().Export();
            ctx.ForType<FooImplementation2>().Export();
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }

        [TestMethod]
        public void WhereNullArgument_ShouldThrowArgumentException()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("typeFilter", () =>
            {
            var ctx = new RegistrationBuilder();
                ctx.ForTypesMatching( null );
                var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
                Assert.IsTrue(catalog.Parts.Count() == 0);
            });
        }

        [TestMethod]
        public void WhereIsClassAndImplementsIFooNoExport_ShouldFindZeroParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesMatching( (t) => { return t.IsClass && typeof(IFoo).IsAssignableFrom(t); } );                      // Implements<IFoo>
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 0);
        }

        [TestMethod]
        public void WhereIsClassAndImplementsIFooWithExport_ShouldFindTwoParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesMatching( (t) => { return t.IsClass && typeof(IFoo).IsAssignableFrom(t); } ).Export();                      // Implements<IFoo>
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 2);
        }

        [TestMethod]
        public void WhereIsTypeWithExport_ShouldFindTwoParts()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesMatching( (t) => { return t.IsAssignableFrom(typeof(FooImplementation1)); } ).Export();                  // Implements<FooImplementation1>
            ctx.ForTypesMatching( (t) => { return t.IsAssignableFrom(typeof(FooImplementation2)); } ).Export();                  // Implements<FooImplementation2>
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(IFoo), typeof(FooImplementation1), typeof(FooImplementation2)),ctx); 
            Assert.IsTrue(catalog.Parts.Count() == 2);
        }
    }
}
