using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Composition.UnitTests.Util;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class ConventionBuilderCompatibilityTests
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
            var rb = new ConventionBuilder();
            var c = new ContainerConfiguration()
                .WithPart(typeof(Derived), rb)
                .CreateContainer();
        }

        public interface IRepository<T> { }

        public class EFRepository<T> : IRepository<T> { }


        [TestMethod]
        public void ConventionBuilderExportsOpenGenerics()
        {
            var rb = new ConventionBuilder();

            rb.ForTypesDerivedFrom(typeof(IRepository<>))
                .Export(eb => eb.AsContractType(typeof(IRepository<>)));

            var c = new ContainerConfiguration()
                .WithPart(typeof(EFRepository<>), rb)
                .CreateContainer();

            var r = c.GetExport<IRepository<string>>();
        }

        public class Imported { }

        public class BaseWithImport
        {
            public virtual Imported Imported { get; set; }
        }

        public class DerivedFromBaseWithImport : BaseWithImport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyImportsToInheritedProperties()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<Imported>().Export();
            conventions.ForType<DerivedFromBaseWithImport>()
                .ImportProperty(b => b.Imported)
                .Export();

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(Imported), typeof(DerivedFromBaseWithImport))
                .CreateContainer();

            var dfb = container.GetExport<DerivedFromBaseWithImport>();
            Assert.IsInstanceOfType(dfb.Imported, typeof(Imported));
        }

        public class BaseWithExport
        {
            public string Exported { get { return "A"; } }
        }

        public class DerivedFromBaseWithExport : BaseWithExport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyExportsToInheritedProperties()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport>()
                .ExportProperty(b => b.Exported);

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(DerivedFromBaseWithExport))
                .CreateContainer();

            var s = container.GetExport<string>();
            Assert.AreEqual("A", s);
        }

        public class BaseWithExport2
        {
            [Export]
            public virtual string Exported { get { return "A"; } }
        }

        public class DerivedFromBaseWithExport2 : BaseWithExport
        {
        }

        [TestMethod]
        public void ConventionsCanApplyExportsToInheritedPropertiesWithoutInterferingWithBase()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport2>()
                .ExportProperty(b => b.Exported);

            var container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(BaseWithExport2))
                .WithParts(typeof(DerivedFromBaseWithExport2))
                .CreateContainer();

            var s = container.GetExports<string>();
            Assert.AreEqual(2, s.Count());
        }
    }
}
