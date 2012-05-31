using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class CompositionContextExtensionsTests : ContainerTests
    {
        public interface IUnregistered { }

        [TestMethod]
        public void GettingAnOptionalExportThatDoesntExistReturnsNull()
        {
            var c = CreateContainer();

            IUnregistered unregistered;
            Assert.IsFalse(c.TryGetExport<IUnregistered>(null, out unregistered));
            Assert.IsNull(unregistered);
        }
    }
}
