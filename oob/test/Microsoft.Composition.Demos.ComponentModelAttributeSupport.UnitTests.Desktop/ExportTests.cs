using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Composition.Demos.ComponentModelAttributeSupport.UnitTests
{
    [TestClass]
    public class ExportTests
    {
        public interface IBus { }

        [System.ComponentModel.Composition.Export(typeof(IBus))]
        public class CloudBus : IBus { }

        [TestMethod]
        public void ExportAttributeAppliesCorrectContract()
        {
            var container = new ContainerConfiguration()
                .WithPart<CloudBus>(new ComponentModelAttributeConvention())
                .CreateContainer();

            IBus bus;
            Assert.IsTrue(container.TryGetExport(null, out bus));
            Assert.IsInstanceOfType(bus, typeof(CloudBus));
        }

        public class SpecialCloudBus : CloudBus { }

        [TestMethod]
        public void DoesNotApplyExportAttributesFromBase()
        {
            var container = new ContainerConfiguration()
                .WithPart<SpecialCloudBus>(new ComponentModelAttributeConvention())
                .CreateContainer();

            IBus bus;
            Assert.IsFalse(container.TryGetExport(null, out bus));
        }
    }
}
