namespace System.ComponentModel.Composition
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Registration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class ExportBuilderTests
    {
        interface IFoo { }
        
        class FooImpl {}
    
        [TestMethod]
        public void AsContractTypeOfT_SetsContractType()
        {
            var builder = new ExportBuilder();
            builder.AsContractType<IFoo>();
            
            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
            Assert.IsNull(exportAtt.ContractName);
        }
    
        [TestMethod]
        public void AsContractType_SetsContractType()
        {
            var builder = new ExportBuilder();
            builder.AsContractType(typeof(IFoo));
            
            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
            Assert.IsNull(exportAtt.ContractName);
        }
    
        [TestMethod]
        public void AsContractName_SetsContractName()
        {
            var builder = new ExportBuilder();
            builder.AsContractName("hey");
            
            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual("hey", exportAtt.ContractName);
            Assert.IsNull(exportAtt.ContractType);
        }

        [TestMethod]
        public void AsContractName_AndContractType_SetsContractNameAndType()
        {
            var builder = new ExportBuilder();
            builder.AsContractName("hey");
            builder.AsContractType(typeof (IFoo));
            
            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.AreEqual("hey", exportAtt.ContractName);
            Assert.AreEqual(typeof(IFoo), exportAtt.ContractType);
        }

        [TestMethod]
        public void Inherited_AddsInheritedExportAttribute()
        {
            var builder = new ExportBuilder();
            builder.Inherited();

            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(1, list.Count);
            var att = list[0] as InheritedExportAttribute;
            Assert.IsNotNull(att);
        }

        [TestMethod]
        public void AddMetadata_AddsExportMetadataAttribute()
        {
            var builder = new ExportBuilder();
            builder.AddMetadata("name", "val");

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.AreEqual("name", exportAtt.Name);
            Assert.AreEqual("val", exportAtt.Value);
        }

        [TestMethod]
        public void AddMetadataFuncVal_AddsExportMetadataAttribute()
        {
            var builder = new ExportBuilder();
            builder.AddMetadata("name", t => t.Name);

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.AreEqual("name", exportAtt.Name);
            Assert.AreEqual(typeof(FooImpl).Name, exportAtt.Value);
        }

        private static ExportAttribute GetExportAttribute(ExportBuilder builder)
        {
            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(1, list.Count);
            return list[0] as ExportAttribute;
        }

        private static ExportMetadataAttribute GetExportMetadataAttribute(ExportBuilder builder)
        {
            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(2, list.Count);
            return list[1] as ExportMetadataAttribute;
        }
	}
}
