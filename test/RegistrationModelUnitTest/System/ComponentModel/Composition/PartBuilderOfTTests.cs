using System.Collections.Generic;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class PartBuilderOfTTests
    {
        public interface IFirst {}

        interface IFoo { }
        class FooImpl
        {
            public string P1 { get; set; }
            public string P2 { get; set; }
            public IEnumerable<IFoo> P3 { get; set; }
        }
        class FooImplWithConstructors
        {
            public FooImplWithConstructors() { }
            public FooImplWithConstructors(IEnumerable<IFoo> ids) { }
            public FooImplWithConstructors(int id, string name) { }
        }

        [TestMethod]
        public void NoOperations_ShouldGenerateNoAttributes()
        {
            var builder = new PartBuilder<FooImpl>(t => true);

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(0, configuredMembers.Count);
        }

        [TestMethod]
        public void ExportSelf_ShouldGenerateSingleExportAttribute()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.Export();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(0, configuredMembers.Count);
            Assert.AreSame(typeof(ExportAttribute), typeAtts.ElementAt(0).GetType());
            Assert.IsNull((typeAtts.ElementAt(0) as ExportAttribute).ContractType);
            Assert.IsNull((typeAtts.ElementAt(0) as ExportAttribute).ContractName);
        }

        [TestMethod]
        public void ExportOfT_ShouldGenerateSingleExportAttributeWithContractType()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(0, configuredMembers.Count);
            Assert.AreSame(typeof(ExportAttribute), typeAtts.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IFoo), (typeAtts.ElementAt(0) as ExportAttribute).ContractType);
            Assert.IsNull((typeAtts.ElementAt(0) as ExportAttribute).ContractName);
        }

        [TestMethod]
        public void AddMetadata_ShouldGeneratePartMetadataAttribute()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.Export<IFoo>().AddMetadata("name", "value");

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(2, typeAtts.Count());
            Assert.AreEqual(0, configuredMembers.Count);
            Assert.AreSame(typeof(ExportAttribute), typeAtts.ElementAt(0).GetType());
            Assert.IsTrue(typeAtts.ElementAt(0) is ExportAttribute);
            Assert.IsTrue(typeAtts.ElementAt(1) is PartMetadataAttribute);

            var metadataAtt = typeAtts.ElementAt(1) as PartMetadataAttribute;
            Assert.AreEqual("name", metadataAtt.Name);
            Assert.AreEqual("value", metadataAtt.Value);
        }

        [TestMethod]
        public void AddMetadataWithFunc_ShouldGeneratePartMetadataAttribute()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.Export<IFoo>().AddMetadata("name", t => t.Name);

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(2, typeAtts.Count());
            Assert.AreEqual(0, configuredMembers.Count);
            Assert.AreSame(typeof(ExportAttribute), typeAtts.ElementAt(0).GetType());
            Assert.IsTrue(typeAtts.ElementAt(0) is ExportAttribute);
            Assert.IsTrue(typeAtts.ElementAt(1) is PartMetadataAttribute);

            var metadataAtt = typeAtts.ElementAt(1) as PartMetadataAttribute;
            Assert.AreEqual("name", metadataAtt.Name);
            Assert.AreEqual(typeof(FooImpl).Name, metadataAtt.Value);
        }

        [TestMethod]
        public void ExportProperty_ShouldGenerateExportForPropertySelected()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.ExportProperty(p => p.P1).Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(FooImpl).GetProperty("P1"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var expAtt = atts[0] as ExportAttribute;
            Assert.IsNull(expAtt.ContractName);
            Assert.IsNull(expAtt.ContractType);
        }

        [TestMethod]
        public void ImportProperty_ShouldGenerateImportForPropertySelected()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.ImportProperty(p => p.P2).Export<IFoo>(); // P2 is string

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(FooImpl).GetProperty("P2"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var importAttribute = atts[0] as ImportAttribute;
            Assert.IsNotNull(importAttribute);
            Assert.IsNull(importAttribute.ContractName);
            Assert.IsNull(importAttribute.ContractType);
        }

        [TestMethod]
        public void ImportProperty_ShouldGenerateImportForPropertySelected_And_ApplyImportMany()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.ImportProperty(p => p.P3).Export<IFoo>(); // P3 is IEnumerable<IFoo>

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(FooImpl).GetProperty("P3"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var importManyAttribute = atts[0] as ImportManyAttribute;
            Assert.IsNotNull(importManyAttribute);
            Assert.IsNull(importManyAttribute.ContractName);
            Assert.IsNull(importManyAttribute.ContractType);
        }

        [TestMethod]
        public void ExportPropertyWithConfiguration_ShouldGenerateExportForPropertySelected()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.
                ExportProperty(p => p.P1, c => c.AsContractName("hey")).
                Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(FooImpl).GetProperty("P1"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var expAtt = atts[0] as ExportAttribute;
            Assert.AreEqual("hey", expAtt.ContractName);
            Assert.IsNull(expAtt.ContractType);
        }

        [TestMethod]
        public void ExportPropertyOfT_ShouldGenerateExportForPropertySelectedWithTAsContractType()
        {
            var builder = new PartBuilder<FooImpl>(t => true);
            builder.
                ExportProperty<string>(p => p.P1).
                Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts);

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(FooImpl).GetProperty("P1"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var expAtt = atts[0] as ExportAttribute;
            Assert.IsNull(expAtt.ContractName);
            Assert.AreEqual(typeof(string), expAtt.ContractType);
        }

        [TestMethod]
        public void ConventionSelectsConstructor_SelectsTheOneWithMostParameters()
        {
            var builder = new PartBuilder<FooImplWithConstructors>(t => true);
            builder.Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(FooImplWithConstructors));

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(3, configuredMembers.Count);

            var tuple = configuredMembers[0]; // Constructor
            var ci = typeof(FooImplWithConstructors).GetConstructors()[2];
            Assert.IsTrue(tuple.Item1 is ConstructorInfo);
            Assert.AreSame(ci, tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
            Assert.IsTrue(tuple.Item2[0] is ImportingConstructorAttribute);

            tuple = configuredMembers[1]; // Parameter 1
            Assert.IsTrue(tuple.Item1 is ParameterInfo);
            Assert.AreSame(ci.GetParameters()[0], tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
            Assert.IsTrue(tuple.Item2[0] is ImportAttribute);

            tuple = configuredMembers[2]; // Parameter 2
            Assert.IsTrue(tuple.Item1 is ParameterInfo);
            Assert.AreSame(ci.GetParameters()[1], tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
            Assert.IsTrue(tuple.Item2[0] is ImportAttribute);
        }

        [TestMethod]
        public void ManuallySelectingConstructor_SelectsTheExplicitOne()
        {
            var builder = new PartBuilder<FooImplWithConstructors>(t => true);
            builder.
                SelectConstructor(param => new FooImplWithConstructors(param.Import<IEnumerable<IFoo>>())).
                Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(FooImplWithConstructors));

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(2, configuredMembers.Count);

            var tuple = configuredMembers[0]; // Constructor
            var ci = typeof(FooImplWithConstructors).GetConstructors().Where(c => c.GetParameters().Length == 1).Single();
            Assert.IsTrue(tuple.Item1 is ConstructorInfo);
            Assert.AreSame(ci, tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
            Assert.IsTrue(tuple.Item2[0] is ImportingConstructorAttribute);

            tuple = configuredMembers[1]; // Parameter 1
            Assert.IsTrue(tuple.Item1 is ParameterInfo);
            Assert.AreSame(ci.GetParameters()[0], tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
        }

        [TestMethod]
        public void ManuallySelectingConstructor_SelectsTheExplicitOne_IEnumerableParameterBecomesImportMany()
        {
            var builder = new PartBuilder<FooImplWithConstructors>(t => true);
            builder.
                SelectConstructor(param => new FooImplWithConstructors(param.Import<IEnumerable<IFoo>>())).
                Export<IFoo>();

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(FooImplWithConstructors));

            Assert.AreEqual(1, typeAtts.Count());
            Assert.AreEqual(2, configuredMembers.Count);

            var ci = typeof(FooImplWithConstructors).GetConstructors().Where(c => c.GetParameters().Length == 1).Single();

            var tuple = configuredMembers[1]; // Parameter 1
            Assert.IsTrue(tuple.Item1 is ParameterInfo);
            Assert.AreSame(ci.GetParameters()[0], tuple.Item1);
            Assert.AreEqual(1, tuple.Item2.Count);
            Assert.AreEqual(typeof(ImportManyAttribute), tuple.Item2[0].GetType());
        }

        private static void GetConfiguredMembers(PartBuilder builder,
            out List<Tuple<object, List<Attribute>>> configuredMembers, out IEnumerable<Attribute> typeAtts,
            Type targetType = null)
        {
            if (targetType == null)
            {
                targetType = typeof(FooImpl);
            }

            configuredMembers = new List<Tuple<object, List<Attribute>>>();
            typeAtts = builder.BuildTypeAttributes(targetType);
            if (!builder.BuildConstructorAttributes(targetType, ref configuredMembers))
            {
                PartBuilder.BuildDefaultConstructorAttributes(targetType, ref configuredMembers);
            }
            builder.BuildPropertyAttributes(targetType, ref configuredMembers);
        }

        [TestMethod]
        public void ExportInterfaceSelectorNull_ShouldThrowArgumentNull()
        {
            //Same test as above only using default export builder
            var builder = new RegistrationBuilder();
            ExceptionAssert.ThrowsArgumentNull("interfaceFilter", () => builder.ForTypesMatching( (t) => true ).ExportInterfaces(null) );
            ExceptionAssert.ThrowsArgumentNull("interfaceFilter", () => builder.ForTypesMatching( (t) => true ).ExportInterfaces(null, null) );
        }

        [TestMethod]
        public void ImportSelectorNull_ShouldThrowArgumentNull()
        {
            //Same test as above only using default export builder
            var builder = new RegistrationBuilder();
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ImportProperty(null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ImportProperty(null, null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ImportProperty<IFirst>(null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ImportProperty<IFirst>(null, null) );
        }

        [TestMethod]
        public void ExportSelectorNull_ShouldThrowArgumentNull()
        {
            //Same test as above only using default export builder
            var builder = new RegistrationBuilder();
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ExportProperty(null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ExportProperty(null, null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ExportProperty<IFirst>(null) );
            ExceptionAssert.ThrowsArgumentNull("propertyFilter", () => builder.ForTypesMatching<IFoo>( (t) => true ).ExportProperty<IFirst>(null, null) );
        }
    }
}
