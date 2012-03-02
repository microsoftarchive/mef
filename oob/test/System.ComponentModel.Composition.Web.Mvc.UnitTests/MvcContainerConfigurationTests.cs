using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight;
using System.ComponentModel.Composition.Web.Mvc.Internal;
using System.Reflection;
using System.ComponentModel.Composition.Web.Mvc.UnitTests.MvcContainerConfigurationScenario.Parts;
using System.Web.Mvc;
using System.ComponentModel.Composition.Web.Mvc.UnitTests.MvcContainerConfigurationScenario.Controllers;
using System.ComponentModel.Composition.Web.Mvc.UnitTests.MvcContainerConfigurationScenario;
using System.ComponentModel.Composition.Web.Mvc.UnitTests.Util;

namespace System.ComponentModel.Composition.Web.Mvc.UnitTests
{
    namespace MvcContainerConfigurationScenario
    {
        namespace Parts
        {
        }

        namespace Controllers
        {            
            public class SimpleController : Controller { }
        }
    }

    [TestClass]
    public class MvcContainerConfigurationTests
    {
        RequestScopeFactory CreateRequestScopeFactory(params Type[] partTypes)
        {
            var container = ConfigureContainer(partTypes);
            return container.Value.GetExport<RequestScopeFactory>();
        }

        ExportLifetimeContext<IExportProvider> ConfigureContainer(params Type[] partTypes)
        {
            var configuration = new MvcContainerConfiguration(new Assembly[0])
                .WithParts(partTypes)
                .WithPart<RequestScopeFactory>();

            return configuration.CreateContainer();
        }

        [TestMethod]
        public void ControllersAreExportedAndNonShared()
        {
            var rsf = CreateRequestScopeFactory(typeof(SimpleController));

            var r1 = rsf.BeginRequestScope();
            var r1sp1 = r1.Value.GetExport<SimpleController>();
            var r1sp2 = r1.Value.GetExport<SimpleController>();

            Assert.AreNotSame(r1sp1, r1sp2);
        }

        [TestMethod, Ignore]
        public void ModelBindersAreExportedUnderSpecificContract()
        {
        }
    }
}
