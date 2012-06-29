using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace System.Composition.UnitTests
{
    [TestClass]
    public class PropertyExportTests : ContainerTests
    {
        public class Messenger
        {
            [Export]
            public string Message { get { return "Helo!"; } }
        }

        [TestMethod]
        public void CanExportProperty()
        {
            var cc = CreateContainer(typeof(Messenger));

            var x = cc.GetExport<string>();

            Assert.AreEqual("Helo!", x);
        }

        [Export, Shared]
        public class SelfObsessed
        {
            [Export]
            public SelfObsessed Self { get { return this; } }
        }

        [Export]
        public class Selfless
        {
            [ImportMany]
            public IList<SelfObsessed> Values { get; set; }
        }

        [TestMethod]
        public void ExportedPropertiesShareTheSameSharedPartInstance()
        {
            var cc = CreateContainer(typeof(SelfObsessed), typeof(Selfless));
            var sl = cc.GetExport<Selfless>();
            Assert.AreSame(sl.Values[0], sl.Values[1]);
        }
    }
}
