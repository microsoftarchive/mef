using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class MetadataConstrainedDiscriminatorTests
    {
        [TestMethod]
        public void DiscriminatorsWithEquivalentKeysAndValuesAreEqual()
        {
            var mcd1 = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", new[] { "B" } } });
            var mcd2 = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", new[] { "B" } } });
            Assert.IsTrue(mcd1.Equals(mcd2));
        }

        [TestMethod]
        public void DiscriminatorsWithEquivalentKeysAndValuesHaveTheSameHashCode()
        {
            var mcd1 = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", new[] { "B" } } });
            var mcd2 = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", new[] { "B" } } });
            Assert.AreEqual(mcd1.GetHashCode(), mcd2.GetHashCode());
        }

        [TestMethod]
        public void FormattingTheDiscriminatorWithNoInnerDiscriminatorPrintsKeysAndValues()
        {
            var mcd = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", 1 }, { "B", "C" } });
            var s = mcd.ToString();
            Assert.AreEqual("{ A = 1, B = \"C\" }", s);
        }

        [TestMethod]
        public void FormattingTheDiscriminatorWithAnInnerDiscriminatorPrintsInnerAsString()
        {
            var mcd = new MetadataConstrainedDiscriminator(new Dictionary<string, object> { { "A", 1 } }, "inner");
            var s = mcd.ToString();
            Assert.AreEqual("\"inner\" { A = 1 }", s);
        }
    }
}
