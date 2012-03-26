using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class RegistrationBuilderCompatibilityTests
    {
        public class Base
        {
            public object Prop { get; set; }
        }

        [Export]
        public class Derived : Base
        {
            new public string Prop { get; set; }
        }

        [TestMethod]
        public void WhenConventionsAreInUseDuplicatePropertyNamesDoNotBreakDiscovery()
        {
            var rb = new RegistrationBuilder();
            var c = new ContainerConfiguration()
                .WithPart(typeof(Derived), rb)
                .CreateContainer();
        }

        public interface IRepository<T> { }

        public class EFRepository<T> : IRepository<T> { }


        // Seems to be a problem with RegistrationBuilder, investigating.
        [TestMethod, Ignore]
        public void RegistrationBuilderExportsOpenGenerics()
        {
            var rb = new RegistrationBuilder();

            rb.ForTypesDerivedFrom(typeof(IRepository<>))
                .Export(eb => eb.AsContractType(typeof(IRepository<>)));

            var c = new ContainerConfiguration()
                .WithPart(typeof(EFRepository<>), rb)
                .CreateContainer();

            var r = c.Value.GetExport<IRepository<string>>();
        }
    }
}
