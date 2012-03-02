using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class LazyTests : ContainerTests
    {
        public interface IA { }

        [Export(typeof(IA))]
        public class A : IA { }

        [Export]
        public class BLazy
        {
            public Lazy<IA> A;

            [ImportingConstructor]
            public BLazy(Lazy<IA> ia)
            {
                A = ia;
            }
        }

        [Export, ExportMetadata("Name", "Fred")]
        public class NamedFred { }

        public interface INamed { string Name { get; } }
        [TestMethod]
        public void ComposesLazily()
        {
            var cc = CreateContainer(typeof(A), typeof(BLazy));
            var x = cc.GetExport<BLazy>();
            Assert.IsInstanceOfType(x.A.Value, typeof(A));
        }

        [TestMethod]
        public void SupportsExportMetadata()
        {
            var cc = CreateContainer(typeof(NamedFred));
            var fred = cc.GetExport<Lazy<NamedFred, INamed>>();
            Assert.AreEqual("Fred", fred.Metadata.Name);
        }
    }
}
