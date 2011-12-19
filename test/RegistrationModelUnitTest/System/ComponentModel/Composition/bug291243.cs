using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Bug291243Repro
{
    class RealPart
    {
    }

    class DiscoveredCatalog : AssemblyCatalog
    {
        public DiscoveredCatalog()
            : base("") { }
    }

    [TestClass]
    public class Bug291243
    {
        [TestMethod]
        public void ShouldSucceed()
        {
            var rb = new RegistrationBuilder();
            rb.ForType<RealPart>().Export();
            var cat = new AssemblyCatalog(typeof(Bug291243).Assembly, rb);
            var container = new CompositionContainer(cat);

            // Throws:
            // Can not determine which constructor to use for the type 'System.ComponentModel.Composition.Hosting.AssemblyCatalog, System.ComponentModel.Composition, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'.
            var rp = container.GetExport<RealPart>().Value;
        }
    }
}
