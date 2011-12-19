namespace System.ComponentModel.Composition
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Registration;
	using System.Linq;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class PartBuilderInheritanceTests
	{
		abstract class BaseClass
		{
			public string P1 { get; set; }
			public string P2 { get; set; }
			public IEnumerable<int> P3 { get; set; }
		}

		class DerClass : BaseClass
		{
			public string P4 { get; set; }
			public string P5 { get; set; }
		}

        [TestMethod]
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportForPropertySelected()
        {
            var builder = new PartBuilder(t => true);
            builder.ImportProperties(p => p.Name == "P2"); // P2 is string

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(DerClass));

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(BaseClass).GetProperty("P2"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var importAttribute = atts[0] as ImportAttribute;
            Assert.IsNotNull(importAttribute);
            Assert.IsNull(importAttribute.ContractName);
            Assert.IsNull(importAttribute.ContractType);
        }

        [TestMethod]
        public void ImportPropertyTargetingBaseClass_ShouldGenerateImportManyForPropertySelected()
        {
            var builder = new PartBuilder(t => true);
            builder.ImportProperties(p => p.Name == "P3"); // P3 is IEnumerable<int>

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(DerClass));

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(BaseClass).GetProperty("P3"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var importManyAttribute = atts[0] as ImportManyAttribute;
            Assert.IsNotNull(importManyAttribute);
            Assert.IsNull(importManyAttribute.ContractName);
            Assert.IsNull(importManyAttribute.ContractType);
        }

        [TestMethod]
        public void ImportPropertyTargetingDerivedClass_ShouldGenerateImportForPropertySelected()
        {
            var builder = new PartBuilder(t => true);
            builder.ImportProperties(p => p.Name == "P4"); // P4 is string

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(DerClass));

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(DerClass).GetProperty("P4"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var importAttribute = atts[0] as ImportAttribute;
            Assert.IsNotNull(importAttribute);
            Assert.IsNull(importAttribute.ContractName);
            Assert.IsNull(importAttribute.ContractType);
        }

        [TestMethod]
        public void ExportPropertyTargetingDerivedClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new PartBuilder(t => true);
            builder.ExportProperties(p => p.Name == "P4"); // P4 is string

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(DerClass));

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(DerClass).GetProperty("P4"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var exportAttribute = atts[0] as ExportAttribute;
            Assert.IsNotNull(exportAttribute);
            Assert.IsNull(exportAttribute.ContractName);
            Assert.IsNull(exportAttribute.ContractType);
        }

        [TestMethod]
        public void ExportPropertyTargetingBaseClass_ShouldGenerateExportForPropertySelected()
        {
            var builder = new PartBuilder(t => true);
            builder.ExportProperties(p => p.Name == "P2"); // P2 is string

            IEnumerable<Attribute> typeAtts;
            List<Tuple<object, List<Attribute>>> configuredMembers;
            GetConfiguredMembers(builder, out configuredMembers, out typeAtts, typeof(DerClass));

            Assert.AreEqual(0, typeAtts.Count());
            Assert.AreEqual(1, configuredMembers.Count);

            var tuple = configuredMembers[0];
            Assert.AreEqual(typeof(BaseClass).GetProperty("P2"), tuple.Item1);

            var atts = tuple.Item2;
            Assert.AreEqual(1, atts.Count);

            var exportAttribute = atts[0] as ExportAttribute;
            Assert.IsNotNull(exportAttribute);
            Assert.IsNull(exportAttribute.ContractName);
            Assert.IsNull(exportAttribute.ContractType);
        }


        private static void GetConfiguredMembers(PartBuilder builder,
            out List<Tuple<object, List<Attribute>>> configuredMembers, out IEnumerable<Attribute> typeAtts,
            Type targetType)
        {
            configuredMembers = new List<Tuple<object, List<Attribute>>>();
            typeAtts = builder.BuildTypeAttributes(targetType);
            builder.BuildConstructorAttributes(targetType, ref configuredMembers);
            builder.BuildPropertyAttributes(targetType, ref configuredMembers);
        }
	}
}
