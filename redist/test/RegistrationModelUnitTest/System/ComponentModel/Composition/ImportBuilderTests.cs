namespace System.ComponentModel.Composition
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Registration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImportBuilderTests
    {
        interface IFoo { }
        
        class FooImpl { }
        
        [TestMethod]
        public void AsContractTypeOfT_SetsContractType()
        {
            var builder = new ImportBuilder();
            builder.AsContractType<IFoo>();
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), importAtt.ContractType);
            Assert.IsNull(importAtt.ContractName);
            Assert.IsFalse(importAtt.AllowDefault);
            Assert.IsFalse(importAtt.AllowRecomposition);
        }
        
        [TestMethod]
        public void AsContractType_SetsContractType()
        {
            var builder = new ImportBuilder();
            builder.AsContractType(typeof(IFoo));
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual(typeof(IFoo), importAtt.ContractType);
            Assert.IsNull(importAtt.ContractName);
            Assert.IsFalse(importAtt.AllowDefault);
            Assert.IsFalse(importAtt.AllowRecomposition);
        }

        [TestMethod]
        public void AsContractName_SetsContractName()
        {
            var builder = new ImportBuilder();
            builder.AsContractName("hey");
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual("hey", importAtt.ContractName);
            Assert.IsFalse(importAtt.AllowDefault);
            Assert.IsFalse(importAtt.AllowRecomposition);
            Assert.IsNull(importAtt.ContractType);
    }

        [TestMethod]
        public void RequiredCreationPolicy_SetsRequiredCreationPolicyProperty()
        {
            var builder = new ImportBuilder();
            builder.RequiredCreationPolicy(CreationPolicy.NonShared);
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual(CreationPolicy.NonShared, importAtt.RequiredCreationPolicy);
            Assert.IsFalse(importAtt.AllowDefault);
            Assert.IsFalse(importAtt.AllowRecomposition);
            Assert.IsNull(importAtt.ContractType);
            Assert.IsNull(importAtt.ContractName);
        }

        [TestMethod]
        public void AllowDefault_SetsAllowDefaultProperty()
        {
            var builder = new ImportBuilder();
            builder.AllowDefault();
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.IsTrue(importAtt.AllowDefault);
            Assert.IsFalse(importAtt.AllowRecomposition);
            Assert.IsNull(importAtt.ContractType);
            Assert.IsNull(importAtt.ContractName);
            }

        [TestMethod]
        public void AllowRecomposition_SetsAllowRecompositionProperty()
        {
            var builder = new ImportBuilder();
            builder.AllowRecomposition();
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.IsTrue(importAtt.AllowRecomposition);
            Assert.IsNull(importAtt.ContractType);
            Assert.IsNull(importAtt.ContractName);
        }

        [TestMethod]
        public void AsContractName_AndContractType_SetsContractNameAndType()
        {
            var builder = new ImportBuilder();
            builder.AsContractName("hey");
            builder.AsContractType(typeof(IFoo));
            
            ImportAttribute importAtt = GetImportAttribute(builder);
            Assert.AreEqual("hey", importAtt.ContractName);
            Assert.AreEqual(typeof(IFoo), importAtt.ContractType);
        }

        [TestMethod]
        public void AsMany_ChangesGeneratedAttributeToImportMany()
        {
            var builder = new ImportBuilder();
            builder.AsMany();

            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(1, list.Count);
            var att = list[0] as ImportManyAttribute;
            Assert.IsNotNull(att);
            Assert.IsNull(att.ContractName);
            Assert.IsNull(att.ContractType);
        }

        [TestMethod]
        public void AsMany_And_ContractName_ChangesGeneratedAttributeToImportMany()
        {
            var builder = new ImportBuilder();
            builder.AsContractName("hey");
            builder.AsMany();

            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(1, list.Count);
            var att = list[0] as ImportManyAttribute;
            Assert.IsNotNull(att);
            Assert.AreEqual("hey", att.ContractName);
            Assert.IsNull(att.ContractType);
        }

        private static ImportAttribute GetImportAttribute(ImportBuilder builder)
        {
            var list = new List<Attribute>();
            builder.BuildAttributes(typeof(FooImpl), ref list);
            Assert.AreEqual(1, list.Count);
            return list[0] as ImportAttribute;
        }
    }
}
