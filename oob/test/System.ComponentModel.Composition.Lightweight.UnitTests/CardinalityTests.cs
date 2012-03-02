using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class CardinalityTests : ContainerTests
    {
        public interface ILog { }

        [Export(typeof(ILog))]
        public class LogA : ILog { }

        [Export(typeof(ILog))]
        public class LogB : ILog { }

        [Export]
        public class UsesLog
        {
            [ImportingConstructor]
            public UsesLog(ILog log) { }
        }

        [TestMethod]
        public void RequestingOneWhereMultipleArePresentFails()
        {
            var c = CreateContainer(typeof(LogA), typeof(LogB));
            var x = AssertX.Throws<LightweightCompositionException>(() =>
                c.GetExport<ILog>());
            Assert.IsTrue(x.Message.Contains("LogA"));
            Assert.IsTrue(x.Message.Contains("LogB"));
        }

        [TestMethod]
        public void ImportingOneWhereMultipleArePresentFails()
        {
            var c = CreateContainer(typeof(LogA), typeof(LogB), typeof(UsesLog));
            var x = AssertX.Throws<LightweightCompositionException>(() =>
                c.GetExport<UsesLog>());
            Assert.IsTrue(x.Message.Contains("LogA"));
            Assert.IsTrue(x.Message.Contains("LogB"));
        }
    }
}
