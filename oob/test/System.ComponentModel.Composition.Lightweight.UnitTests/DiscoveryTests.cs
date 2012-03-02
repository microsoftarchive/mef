using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.UnitTests.Util;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class DiscoveryTests : ContainerTests
    {
        public interface IRule { }

        public class RuleExportAttribute : ExportAttribute
        {
            public RuleExportAttribute() : base(typeof(IRule)) { }
        }

        [RuleExport]
        public class UnfairRule : IRule { }

        [Export(typeof(IRule))]
        public class IncompatibleRule { }

        public class IncompatibleRuleProperty
        {
            [Export(typeof(IRule))]
            public string Rule { get; set; }
        }

        [Export, PartNotDiscoverable]
        public class NotDiscoverable { }

        [TestMethod]
        public void DiscoversCustomExportAttributes()
        {
            var container = CreateContainer(typeof(UnfairRule));
            var rule = container.GetExport<IRule>();
            Assert.IsInstanceOfType(rule, typeof(UnfairRule));
        }

        [TestMethod]
        public void DiscoversCustomExportAttributesUnderConventions()
        {
            var container = CreateContainer(new RegistrationBuilder(), typeof(UnfairRule));
            var rule = container.GetExport<IRule>();
            Assert.IsInstanceOfType(rule, typeof(UnfairRule));
        }

        [TestMethod]
        public void InstanceExportsOfIncompatibleContractsAreDetected()
        {
            var x = AssertX.Throws<LightweightCompositionException>(() => CreateContainer(typeof(IncompatibleRule)));
            Assert.AreEqual("Exported contract type 'IRule' is not assignable from part 'IncompatibleRule'.", x.Message);
        }

        [TestMethod]
        public void PropertyExportsOfIncompatibleContractsAreDetected()
        {
            var x = AssertX.Throws<LightweightCompositionException>(() => CreateContainer(typeof(IncompatibleRuleProperty)));
            Assert.AreEqual("Exported contract type 'IRule' is not assignable from property 'Rule' of part 'IncompatibleRuleProperty'.", x.Message);
        }

        [TestMethod]
        public void ANonDiscoverablePartIsIgnored()
        {
            var container = CreateContainer(typeof(NotDiscoverable));
            NotDiscoverable unused;
            Assert.IsFalse(container.TryGetExport(null, out unused));
        }
    }
}
