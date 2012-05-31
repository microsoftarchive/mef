using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition.Hosting;
using System.Composition.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class MetadataViewGenerationTests
    {
        [Export, ExportMetadata("Name", "A")]
        public class HasNameA { }

        public class Named { public string Name { get; set; } }

        [TestMethod]
        public void AConcreteTypeWithWritablePropertiesIsAMetadataView()
        {
            var cc = new ContainerConfiguration()
                        .WithPart<HasNameA>()
                        .CreateContainer();

            var hn = cc.GetExport<Lazy<HasNameA, Named>>();

            Assert.AreEqual("A", hn.Metadata.Name);
        }

        [Export]
        public class HasNoName { }


        public class OptionallyNamed { [DefaultValue("B")] public string Name { get; set; } }

        [TestMethod]
        public void MetadataViewsCanCarryDefaultValues()
        {
            var cc = new ContainerConfiguration()
                        .WithPart<HasNoName>()
                        .CreateContainer();

            var hn = cc.GetExport<Lazy<HasNoName, OptionallyNamed>>();

            Assert.AreEqual("B", hn.Metadata.Name);
        }

        public class DictionaryName
        {
            public DictionaryName(IDictionary<string, object> metadata)
            {
                RetrievedName = (string)metadata["Name"];
            }

            public string RetrievedName { get; set; }
        }

        [TestMethod]
        public void AConcreteTypeWithDictionaryConstructorIsAMetadataView()
        {
            var cc = new ContainerConfiguration()
                        .WithPart<HasNameA>()
                        .CreateContainer();

            var hn = cc.GetExport<Lazy<HasNameA, DictionaryName>>();

            Assert.AreEqual("A", hn.Metadata.RetrievedName);
        }

        public class InvalidConcreteView
        {
            public InvalidConcreteView(string unsupported) { }
        }

        [TestMethod]
        public void AConcreteTypeWithUnsupportedConstructorsCannotBeUsedAsAMetadataView()
        {
            var cc = new ContainerConfiguration()
                        .WithPart<HasNameA>()
                        .CreateContainer();

            var x = AssertX.Throws<CompositionFailedException>(() => cc.GetExport<Lazy<HasNoName, InvalidConcreteView>>());

            Assert.AreEqual("The type 'InvalidConcreteView' cannot be used as a metadata view as it does not have a suitable (parameterless or dictionary) constructor.", x.Message);
        }

        [Export]
        public class HasUnsupportedMetadata { }

        public interface IUnsupportedMetadataView { }

        [Export]
        public class ImportsUnsupportedMetadataView
        {
            [Import]
            public Lazy<HasUnsupportedMetadata, IUnsupportedMetadataView> LazyImport { get; set; }
        }

        // Not aiming for perfect here - just needs to be indicative.
        [TestMethod]
        public void UseOfOldStyleMetadataViewMessageIsDecipherable()
        {
            var container = new ContainerConfiguration()
                .WithParts(typeof(HasUnsupportedMetadata), typeof(ImportsUnsupportedMetadataView))
                .CreateContainer();

            var x = AssertX.Throws<CompositionFailedException>(() => container.GetExport<ImportsUnsupportedMetadataView>());
            Assert.AreEqual("No export was found for the contract 'Func<IDictionary<String, Object>, IUnsupportedMetadataView> \"MetadataViewProvider\"'" + Environment.NewLine +
                " -> required by import 'metadata' of part 'Lazy<HasUnsupportedMetadata, IUnsupportedMetadataView>'" + Environment.NewLine +
                " -> required by import 'LazyImport' of part 'ImportsUnsupportedMetadataView'" + Environment.NewLine +
                " -> required by initial request for contract 'ImportsUnsupportedMetadataView'.", x.Message);
        }
    }
}
