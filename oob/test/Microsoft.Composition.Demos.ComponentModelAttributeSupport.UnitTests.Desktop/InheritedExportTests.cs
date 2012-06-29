using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Composition.Demos.ComponentModelAttributeSupport.UnitTests
{
    [TestClass]
    public class InheritedExportTests
    {
        public interface IBus { }

        [System.ComponentModel.Composition.InheritedExport(typeof(IBus))]
        public class CloudBus : IBus { }

        public class SpecialCloudBus : CloudBus { }

        // Not implemented at this point
        [TestMethod, Ignore]
        public void InheritedExportAttributeAppliesCorrectContract()
        {
            var container = new ContainerConfiguration()
                .WithPart<SpecialCloudBus>(new ComponentModelAttributeConvention())
                .CreateContainer();

            IBus bus;
            Assert.IsTrue(container.TryGetExport(null, out bus));
            Assert.IsInstanceOfType(bus, typeof(SpecialCloudBus));
        }
    }
}
