using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using Microsoft.CLR.UnitTesting;

namespace Microsoft.Composition.Registration
{
    [TestClass]
    public class RegistrationBuilderTests
    {
        interface IFoo { }
        class FooImpl : IFoo
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
            public IEnumerable<IFoo> P3 { get; set; }
        }
        class FooImplWithConstructors : IFoo
        {
            public FooImplWithConstructors() { }
            public FooImplWithConstructors(IEnumerable<IFoo> ids) { }
            public FooImplWithConstructors(int id, string name) { }
        }

        [TestMethod]
        public void MapType_ShouldReturnProjectedAttributesForType()
        {
            var builder = new RegistrationBuilder();

            builder.
                ForTypesDerivedFrom<IFoo>().
                Export<IFoo>();

            var projectedType1 = builder.MapType(typeof(FooImpl).GetTypeInfo());
            var projectedType2 = builder.MapType(typeof(FooImplWithConstructors).GetTypeInfo());
            
            var exports = new List<object>();
            
            exports.AddRange(projectedType1.GetCustomAttributes(typeof(ExportAttribute), false));
            exports.AddRange(projectedType2.GetCustomAttributes(typeof(ExportAttribute), false));
            Assert.AreEqual(2, exports.Count);

            foreach (var exportAttribute in exports)
            {
                Assert.AreEqual(typeof(IFoo), ((ExportAttribute)exportAttribute).ContractType);
                Assert.IsNull(((ExportAttribute)exportAttribute).ContractName);
            }
        }
        
        [TestMethod]
        public void MapType_ConventionSelectedConstructor()
        {
            var builder = new RegistrationBuilder();
        
            builder.
                ForTypesDerivedFrom<IFoo>().
                Export<IFoo>();

            var projectedType1 = builder.MapType(typeof(FooImpl).GetTypeInfo());
            var projectedType2 = builder.MapType(typeof(FooImplWithConstructors).GetTypeInfo());

            // necessary as BuildConventionConstructorAttributes is only called for type level query for attributes
            var typeLevelAttrs = projectedType2.GetCustomAttributes(false);

            var constructor1 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 0).Single();
            var constructor2 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 1).Single();
            var constructor3 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 2).Single();

            Assert.AreEqual(0, constructor1.GetCustomAttributes(false).Length);
            Assert.AreEqual(0, constructor2.GetCustomAttributes(false).Length);

            var ci = constructor3;
            var attrs = ci.GetCustomAttributes(false);
            Assert.AreEqual(1, attrs.Length);
            Assert.AreEqual(typeof(ImportingConstructorAttribute), attrs[0].GetType());
        }
        
        [TestMethod]
        public void MapType_OverridingSelectionOfConventionSelectedConstructor()
        {
            var builder = new RegistrationBuilder();
            
            builder.
                ForTypesDerivedFrom<IFoo>().
                Export<IFoo>();
            
            builder.ForType<FooImplWithConstructors>()
                .SelectConstructor(cis => cis[1]);

            var projectedType1 = builder.MapType(typeof(FooImpl).GetTypeInfo().GetTypeInfo());
            var projectedType2 = builder.MapType(typeof(FooImplWithConstructors).GetTypeInfo().GetTypeInfo());

            var constructor1 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 0).Single();
            var constructor2 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 1).Single();
            var constructor3 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 2).Single(); 


            // necessary as BuildConventionConstructorAttributes is only called for type level query for attributes
            var typeLevelAttrs = projectedType2.GetCustomAttributes(false);
            
            Assert.AreEqual(0, constructor1.GetCustomAttributes(false).Length);
            Assert.AreEqual(0, constructor3.GetCustomAttributes(false).Length);
            
            var ci = constructor2;
            var attrs = ci.GetCustomAttributes(false);
            Assert.AreEqual(1, attrs.Length);
            Assert.AreEqual(typeof(ImportingConstructorAttribute), attrs[0].GetType());
        }
        
        [TestMethod]
        public void MapType_OverridingSelectionOfConventionSelectedConstructor_WithPartBuilderOfT()
        {
            var builder = new RegistrationBuilder();

            builder.
                ForTypesDerivedFrom<IFoo>().
                Export<IFoo>();

            builder.ForType<FooImplWithConstructors>().
                SelectConstructor(param => new FooImplWithConstructors(param.Import<IEnumerable<IFoo>>()));

            var projectedType1 = builder.MapType(typeof(FooImpl).GetTypeInfo().GetTypeInfo());
            var projectedType2 = builder.MapType(typeof(FooImplWithConstructors).GetTypeInfo().GetTypeInfo());
            
            var constructor1 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 0).Single();
            var constructor2 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 1).Single();
            var constructor3 = projectedType2.GetConstructors().Where(c => c.GetParameters().Length == 2).Single(); 
            
            // necessary as BuildConventionConstructorAttributes is only called for type level query for attributes
            var typeLevelAttrs = projectedType2.GetCustomAttributes(false);
            
            Assert.AreEqual(0, constructor1.GetCustomAttributes(false).Length);
            Assert.AreEqual(0, constructor3.GetCustomAttributes(false).Length);
            
            var ci = constructor2;
            var attrs = ci.GetCustomAttributes(false);
            Assert.AreEqual(1, attrs.Length);
            Assert.AreEqual(typeof(ImportingConstructorAttribute), attrs[0].GetType());
        }

        interface IGenericInterface<T> { }

        [Export(typeof(IGenericInterface<>))]
        class ClassExportingInterface<T> : IGenericInterface<T> { }

        [TestMethod]
        public void GenericInterfaceExportInRegistrationBuilder()
        {
            var container = CreateRegistrationBuilderContainer(typeof(ClassExportingInterface<>));
            var v = container.GetExportedValue<IGenericInterface<string>>();
            Assert.IsInstanceOfType(v, typeof(IGenericInterface<string>));
        }

        class GenericBaseClass<T> { }

        [Export(typeof(GenericBaseClass<>))]
        class ClassExportingBaseClass<T> : GenericBaseClass<T> { }

        [TestMethod]
        public void GenericBaseClassExportInRegistrationBuilder()
        {
            var container = CreateRegistrationBuilderContainer(typeof(ClassExportingBaseClass<>));
            var v = container.GetExportedValue<GenericBaseClass<string>>();
            Assert.IsInstanceOfType(v, typeof(GenericBaseClass<string>));
        }

        [Export]
        class GenericClass<T> { }

        [TestMethod]
        public void GenericExportInRegistrationBuilder()
        {
            var container = CreateRegistrationBuilderContainer(typeof(GenericClass<>));
            var v = container.GetExportedValue<GenericClass<string>>();
            Assert.IsInstanceOfType(v, typeof(GenericClass<string>));
        }

        [Export(typeof(ExplicitGenericClass<>))]
        class ExplicitGenericClass<T> { }

        [TestMethod]
        public void ExplicitGenericExportInRegistrationBuilder()
        {
            var container = CreateRegistrationBuilderContainer(typeof(ExplicitGenericClass<>));
            var v = container.GetExportedValue<ExplicitGenericClass<string>>();
            Assert.IsInstanceOfType(v, typeof(ExplicitGenericClass<string>));
        }

        [Export(typeof(ExplicitGenericClass<,>))]
        class ExplicitGenericClass<T, U> { }

        [TestMethod]
        public void ExplicitGenericArity2ExportInRegistrationBuilder()
        {
            var container = CreateRegistrationBuilderContainer(typeof(ExplicitGenericClass<,>));
            var v = container.GetExportedValue<ExplicitGenericClass<int, string>>();
            Assert.IsInstanceOfType(v, typeof(ExplicitGenericClass<int, string>));
        }

        CompositionContainer CreateRegistrationBuilderContainer(params Type[] types)
        {
            var reg = new RegistrationBuilder();
            var container = new CompositionContainer(new TypeCatalog(types, reg));
            return container;
        }
    }
}
