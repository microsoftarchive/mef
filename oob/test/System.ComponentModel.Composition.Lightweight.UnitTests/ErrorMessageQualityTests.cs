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
    public class ErrorMessageQualityTests : ContainerTests
    {
        public class Unregistered { }

        [Export]
        public class UserOfUnregistered
        {
            [Import] public Unregistered Unregistered { get; set; }
        }

        [Export]
        public class CycleA
        {
            [Import] public CycleB B { get; set; }
        }
        
        [Export]
        public class CycleB
        {
            [Import] public CycleC C { get; set; }
        }
        
        [Export]
        public class CycleC
        {
            [Import] public CycleA A { get; set; }
        }

        [Export]
        public class ShouldBeOne { }

        [Export(typeof(ShouldBeOne))]
        public class ButThereIsAnother : ShouldBeOne { }

        [Export]
        public class RequiresOnlyOne
        {
            [Import] public ShouldBeOne One { get; set; }
        }
        
        [TestMethod]
        public void MissingTopLevelExportMessageIsInformative()
        {
            var cc = CreateContainer();
            var x = AssertX.Throws<LightweightCompositionException>(() => cc.GetExport<Unregistered>());
            Assert.AreEqual("No export was found for the contract 'Unregistered'.", x.Message);
        }
        
        [TestMethod]
        public void MissingTopLevelNamedExportMessageIsInformative()
        {
            var cc = CreateContainer();
            var x = AssertX.Throws<LightweightCompositionException>(() => cc.GetExport<Unregistered>("unregistered"));
            Assert.AreEqual("No export was found for the contract 'Unregistered \"unregistered\"'.", x.Message);
        }

        [TestMethod]
        public void MissingDependencyMessageIsInformative()
        {
            var cc = CreateContainer(typeof(UserOfUnregistered));
            var x = AssertX.Throws<LightweightCompositionException>(() => cc.GetExport<UserOfUnregistered>());
            Assert.AreEqual("No export was found for the contract 'Unregistered'" + Environment.NewLine +
                            " -> required by import 'Unregistered' of part 'UserOfUnregistered'" + Environment.NewLine +
                            " -> required by initial request for contract 'UserOfUnregistered'.", x.Message);
        }

        [TestMethod]
        public void CycleMessageIsInformative()
        {
            var cc = CreateContainer(typeof(CycleA), typeof(CycleB), typeof(CycleC));
            var x = AssertX.Throws<LightweightCompositionException>(() => cc.GetExport<CycleA>());
            Assert.AreEqual("Importing part 'CycleA' creates an unsupported cycle" + Environment.NewLine +
                            " -> required by import 'A' of part 'CycleC'" + Environment.NewLine +
                            " -> required by import 'C' of part 'CycleB'" + Environment.NewLine +
                            " -> required by import 'B' of part 'CycleA'" + Environment.NewLine +
                            " -> required by initial request for contract 'CycleA'." + Environment.NewLine +
                            "To construct a cycle, at least one part in the cycle must be shared, and at least one import in the cycle must be non-prerequisite (e.g. a property).", x.Message);
        }

        [TestMethod]
        public void CardinalityViolationMessageIsInformative()
        {
            var cc = CreateContainer(typeof(ShouldBeOne), typeof(ButThereIsAnother), typeof(RequiresOnlyOne));
            var x = AssertX.Throws<LightweightCompositionException>(() => cc.GetExport<RequiresOnlyOne>());
            Assert.AreEqual("Only one implementation of the contract 'ShouldBeOne' is allowed, but parts 'ButThereIsAnother' and 'ShouldBeOne' export it" + Environment.NewLine +
                            " -> required by import 'One' of part 'RequiresOnlyOne'" + Environment.NewLine +
                            " -> required by initial request for contract 'RequiresOnlyOne'.", x.Message);
        }
    }
}
