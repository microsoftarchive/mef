using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class ExportMetadataDiscoveryTests : ContainerTests
    {
        [MetadataAttribute]
        public class ExportWithNameFooAttribute : ExportAttribute
        {
            public string Name { get { return "Foo"; } }
        }

        [MetadataAttribute]
        public class NameFooAttribute : Attribute
        {
            public string Name { get { return "Foo"; } }
        }

        [ExportWithNameFoo]
        public class SingleNamedExport { }

        [Export, NameFoo]
        public class NamedWithCustomMetadata { }

        [ExportWithNameFoo, ExportMetadata("Priority", 10)]
        public class NamedAndPrioritized { }

        [ExportWithNameFoo, Export, ExportMetadata("Priority", 10)]
        public class MultipleExportsOneNamedAndBothPrioritized { }

        public interface INamed { [DefaultValue(null)] string Name { get; } }

        public interface IMultiValuedName { string[] Name { get; } }

        public interface IPrioritized { [DefaultValue(0)] int Priority { get; } }

        [Export,
         ExportMetadata("Name", "A"),
         ExportMetadata("Name", "B"),
         ExportMetadata("Name", "B")]
        public class MultipleNames { }

        [TestMethod]
        public void DiscoversMetadataSpecifiedUsingMetadataAttributeOnExportAttribute()
        {
            var cc = CreateContainer(typeof(SingleNamedExport));
            var ne = cc.GetExport<Lazy<SingleNamedExport, INamed>>();
            Assert.AreEqual("Foo", ne.Metadata.Name);
        }

        [TestMethod]
        public void IfMetadataIsSpecifiedOnAnExportAttributeOtherExportsDoNotHaveIt()
        {
            var cc = CreateContainer(typeof(MultipleExportsOneNamedAndBothPrioritized));
            var ne = cc.GetExports<Lazy<MultipleExportsOneNamedAndBothPrioritized, INamed>>();
            Assert.AreEqual(2, ne.Count());
            Assert.IsTrue(ne.Where(e => e.Metadata.Name != null).Count() == 1);
        }

        [TestMethod]
        public void DiscoversStandaloneExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedAndPrioritized));
            var ne = cc.GetExport<Lazy<NamedAndPrioritized, IPrioritized>>();
            Assert.AreEqual(10, ne.Metadata.Priority);
        }

        [TestMethod]
        public void DiscoversStandaloneExportMetadataUsingMetadataAttributes()
        {
            var cc = CreateContainer(typeof(NamedWithCustomMetadata));
            var ne = cc.GetExport<Lazy<NamedWithCustomMetadata, INamed>>();
            Assert.AreEqual("Foo", ne.Metadata.Name);
        }

        [TestMethod]
        public void StandaloneExportMetadataAppliesToAllExportsOnAMember()
        {
            var cc = CreateContainer(typeof(MultipleExportsOneNamedAndBothPrioritized));
            var ne = cc.GetExports<Lazy<MultipleExportsOneNamedAndBothPrioritized, IPrioritized>>();
            Assert.AreEqual(2, ne.Count());
            Assert.IsTrue(ne.All(e => e.Metadata.Priority == 10));
        }

        [TestMethod]
        public void MultiplePiecesOfMetadataAreCombinedIntoAnArray()
        {
            var cc = CreateContainer(typeof(MultipleNames));

            var withNames = cc.GetExport<Lazy<MultipleNames, IMultiValuedName>>();

            CollectionAssert.AreEquivalent(new[] { "A", "B", "B" }, withNames.Metadata.Name);
        }
        
        [Export]
        public class ConstructorImported { }

        [Export("A"), Export("B")]
        public class MultipleExportsNonDefaultConstructor
        {
            [ImportingConstructor]
            public MultipleExportsNonDefaultConstructor(ConstructorImported c) { }
        }

        [TestMethod]
        public void MultipleExportsCanBeRetrievedWhenANonDefaultConstructorExists()
        {
            var c = CreateContainer(typeof(ConstructorImported), typeof(MultipleExportsNonDefaultConstructor));
            c.GetExport<MultipleExportsNonDefaultConstructor>("A");
            c.GetExport<MultipleExportsNonDefaultConstructor>("B");
        }
    }
}
