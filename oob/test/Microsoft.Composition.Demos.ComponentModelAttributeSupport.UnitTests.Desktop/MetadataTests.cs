using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Composition;

namespace Microsoft.Composition.Demos.ComponentModelAttributeSupport.UnitTests
{
    [TestClass]
    public class MetadataTests : ContainerTests
    {
        public interface ICircularM { string Name { get; } }

        [Export, ExportMetadata("Name", "A")]
        public class MetadataCircularityA
        {
            [Import]
            public Lazy<MetadataCircularityB, ICircularM> B { get; set; }
        }

        [Export, ExportMetadata("Name", "B"), Shared]
        public class MetadataCircularityB
        {
            [Import]
            public Lazy<MetadataCircularityA, ICircularM> A { get; set; }
        }

        [TestMethod]
        public void HandlesMetadataCircularity()
        {
            var cc = CreateContainer(typeof(MetadataCircularityA), typeof(MetadataCircularityB));
            var a = cc.GetExport<MetadataCircularityA>();

            Assert.AreEqual(a.B.Metadata.Name, "B");
            Assert.AreEqual(a.B.Value.A.Metadata.Name, "A");
        }


        [MetadataAttribute]
        public class ExportWithNameFooAttribute : ExportAttribute
        {
            public string Name { get { return "Foo"; } }
        }

        [ExportWithNameFoo]
        public class SingleNamedExport { }

        public interface INamed { [DefaultValue(null)] string Name { get; } }

        [ExportWithNameFoo, Export, ExportMetadata("Priority", 10)]
        public class MultipleExportsOneNamedAndBothPrioritized { }

        [ExportWithNameFoo, ExportMetadata("Priority", 10)]
        public class NamedAndPrioritized { }

        [MetadataAttribute]
        public class NameFooAttribute : Attribute
        {
            public string Name { get { return "Foo"; } }
        }

        [Export,
         ExportMetadata("Name", "A"),
         ExportMetadata("Name", "B"),
         ExportMetadata("Name", "B")]
        public class MultipleNames { }

        [Export, NameFoo]
        public class NamedWithCustomMetadata { }

        public interface IMultiValuedName { string[] Name { get; } }

        public interface IPrioritized { [DefaultValue(0)] int Priority { get; } }

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

        [Export, ExportMetadata("Name", "Fred")]
        public class NamedFred { }

        [TestMethod]
        public void SupportsExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedFred));
            var fred = cc.GetExport<Lazy<NamedFred, INamed>>();
            Assert.AreEqual("Fred", fred.Metadata.Name);
        }

    }
}
