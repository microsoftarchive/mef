using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class ContractTests
    {
        class AType { }

        [TestMethod]
        public void FormattingAContractWithNoDiscriminatorShowsTheSimpleTypeName()
        {
            var c = new Contract(typeof(AType));
            var s = c.ToString();
            Assert.AreEqual("AType", s);
        }

        [TestMethod]
        public void FormattingAContractWithAStringDiscriminatorShowsTheDiscriminatorInQuotes()
        {
            var c = new Contract(typeof(AType), "at");
            var s = c.ToString();
            Assert.AreEqual("AType \"at\"", s);
        }

        [TestMethod]
        public void FormattingAContractWithNonStringDiscriminatorShowsTheDiscriminatorLiterally()
        {
            var c = new Contract(typeof(AType), 1);
            var s = c.ToString();
            Assert.AreEqual("AType 1", s);
        }
    }
}
