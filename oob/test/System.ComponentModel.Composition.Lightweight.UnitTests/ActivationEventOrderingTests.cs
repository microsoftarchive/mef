using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Lightweight.Hosting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [Export]
    public class Imported { }

    [Export]
    public class TracksImportSatisfaction : IPartImportsSatisfiedNotification
    {
        [Import]
        public Imported Imported { get; set; }

        public Imported SetOnImportsSatisfied { get; set; }

        public void OnImportsSatisfied()
        {
            SetOnImportsSatisfied = Imported;
        }
    }

    [TestClass]
    public class ActivationEventOrderingTests : ContainerTests
    {
        [TestMethod]
        public void OnImportsSatisfiedIsCalledAfterPropertyInjection()
        {
            var cc = CreateContainer(typeof(TracksImportSatisfaction), typeof(Imported));

            var tis = cc.GetExport<TracksImportSatisfaction>();

            Assert.IsNotNull(tis.SetOnImportsSatisfied);
        }
    }
}
