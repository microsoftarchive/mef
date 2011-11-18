using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Linq;
using System.Text;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Registration
{
    [TestClass]
    public class ExportBuilderUnitTests
    {
        public interface IFoo {}

        public class CFoo : IFoo {}
        public class FooImporter
        {
            [Import]
            public IFoo fooImporter { get; set; }
        }

        public class CFoo1 : IFoo  {}
        public class CFoo2 : IFoo  {}
        public class CFoo3 : IFoo  {}
        public class CFoo4 : IFoo  {}

        public class FooImporterBase
        {
            public IFoo ImportedFoo { get; set; }
            public IEnumerable<IFoo> IFoos { get; set; }
        }

        public class FooImporter1 : FooImporterBase 
        {
            public FooImporter1 () {}
            public FooImporter1(IFoo fooImporter)
            {
                ImportedFoo = fooImporter; 
            }
        }

        public class FooImporter2 : FooImporterBase
        {
            public FooImporter2 () {}
            public FooImporter2(IFoo fooImporter)
            {
                ImportedFoo = fooImporter; 
            }
        }

        public class FooImporter3 : FooImporterBase
        {
            public FooImporter3 () {}
            public FooImporter3(IFoo fooImporter)
            {
                ImportedFoo = fooImporter; 
            }
        }

        public class FooImporter4 : FooImporterBase
        {
            public FooImporter4 () {}
            public FooImporter4(IFoo fooImporter)
            {
                ImportedFoo = fooImporter; 
            }
        }

        [TestMethod]
        public void ExportInterfaceWithTypeOf1()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo>().Export<IFoo>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() != 0);
            
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var importer = new FooImporter();
            container.SatisfyImportsOnce(importer);

            Assert.IsNotNull(importer.fooImporter, "fooImporter not set!");
        }

        [TestMethod]
        public void ExportInterfaceWithTypeOf2()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType(typeof(CFoo)).Export( (c) => c.AsContractType(typeof(IFoo)));
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() != 0);
            
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var importer = new FooImporter();
            container.SatisfyImportsOnce(importer);

            Assert.IsNotNull(importer.fooImporter, "fooImporter not set!");
        }

        [TestMethod]
        public void ExportInheritedInterfaceWithImplements1()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesDerivedFrom<IFoo>().Export( (c) => c.Inherited().AsContractType(typeof(IFoo)));
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() != 0);
            
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var importer = new FooImporter();
            container.SatisfyImportsOnce(importer);

            Assert.IsNotNull(importer.fooImporter, "fooImporter not set!");
        }

        [TestMethod]
        public void ExportInheritedInterfaceWithImplements2()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForTypesDerivedFrom(typeof(IFoo)).Export( (c) => c.Inherited().AsContractType(typeof(IFoo)));
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(CFoo)), ctx); 
            Assert.IsTrue(catalog.Parts.Count() != 0);
            
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            var importer = new FooImporter();
            container.SatisfyImportsOnce(importer);

            Assert.IsNotNull(importer.fooImporter, "fooImporter not set!");
        }

        [TestMethod]
        public void ComplexNamedComposition()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo1>().Export<IFoo>( (c) => c.Named("one") );
            ctx.ForType<CFoo2>().Export<IFoo>( (c) => c.Named("two") );
            ctx.ForType<CFoo3>().Export<IFoo>( (c) => c.Named("three") );
            ctx.ForType<CFoo4>().Export<IFoo>( (c) => c.Named("four") );

            ctx.ForType<FooImporterBase>().ImportProperties<IFoo>( (pi) => pi.Name == "IFoos", (pi, c) => c.AsMany() );
            ctx.ForType<FooImporter1>().SelectConstructor(null, (pi, ib) => ib.Named("one").AsContractType<IFoo>() ).Export();
            ctx.ForType<FooImporter2>().SelectConstructor(null, (pi, ib) => ib.Named("two").AsContractType<IFoo>() ).Export();
            ctx.ForType<FooImporter3>().SelectConstructor(null, (pi, ib) => ib.Named("three").AsContractType<IFoo>() ).Export();
            ctx.ForType<FooImporter4>().SelectConstructor(null, (pi, ib) => ib.Named("four").AsContractType<IFoo>() ).Export();

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(CFoo1), typeof(CFoo2), typeof(CFoo3), typeof(CFoo4), 
                typeof(FooImporter1), typeof(FooImporter2), typeof(FooImporter3), typeof(FooImporter4)
                ), ctx); 
            
            Assert.IsNotNull(catalog.Parts.Count() == 8, "fooImporter not set!");

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            var one = container.GetExportedValue<FooImporter1>();
            var two = container.GetExportedValue<FooImporter2>();
            var three = container.GetExportedValue<FooImporter3>();
            var four = container.GetExportedValue<FooImporter4>();

            Assert.IsNotNull(one.ImportedFoo != null, "one.fooImporter not set!");
            Assert.IsTrue(one.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(two.ImportedFoo != null, "two.fooImporter not set!");
            Assert.IsTrue(two.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(three.ImportedFoo != null, "three.fooImporter not set!");
            Assert.IsTrue(three.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(four.ImportedFoo != null, "four.fooImporter not set!");
            Assert.IsTrue(four.IFoos.Count() == 4, "wrong number in collection");
        }


        [TestMethod]
        public void ComplexNamedCompositionUsingPartBuilderOfT()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo1>().Export<IFoo>( (c) => c.Named("one") );
            ctx.ForType<CFoo2>().Export<IFoo>( (c) => c.Named("two") );
            ctx.ForType<CFoo3>().Export<IFoo>( (c) => c.Named("three") );
            ctx.ForType<CFoo4>().Export<IFoo>( (c) => c.Named("four") );

            ctx.ForType<FooImporterBase>().ImportProperty<IFoo>( (foo) => foo.IFoos, (c) => c.AsMany() );

            ctx.ForType<FooImporter1>().SelectConstructor((ParameterImportBuilder p) => new FooImporter1(p.Import<IFoo>( (b) => b.Named("one") ) ) ).Export();
            ctx.ForType<FooImporter2>().SelectConstructor((ParameterImportBuilder p) => new FooImporter2(p.Import<IFoo>( (b) => b.Named("two") ) ) ).Export();
            ctx.ForType<FooImporter3>().SelectConstructor((ParameterImportBuilder p) => new FooImporter3(p.Import<IFoo>( (b) => b.Named("three") ) ) ).Export();
            ctx.ForType<FooImporter4>().SelectConstructor((ParameterImportBuilder p) => new FooImporter4(p.Import<IFoo>( (b) => b.Named("four") ) ) ).Export();

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(CFoo1), typeof(CFoo2), typeof(CFoo3), typeof(CFoo4), 
                typeof(FooImporter1), typeof(FooImporter2), typeof(FooImporter3), typeof(FooImporter4)
                ), ctx); 
            
            Assert.IsNotNull(catalog.Parts.Count() == 8, "fooImporter not set!");

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            var one = container.GetExportedValue<FooImporter1>();
            var two = container.GetExportedValue<FooImporter2>();
            var three = container.GetExportedValue<FooImporter3>();
            var four = container.GetExportedValue<FooImporter4>();

            Assert.IsNotNull(one.ImportedFoo != null, "one.fooImporter not set!");
            Assert.IsTrue(one.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(two.ImportedFoo != null, "two.fooImporter not set!");
            Assert.IsTrue(two.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(three.ImportedFoo != null, "three.fooImporter not set!");
            Assert.IsTrue(three.IFoos.Count() == 4, "wrong number in collection");

            Assert.IsNotNull(four.ImportedFoo != null, "four.fooImporter not set!");
            Assert.IsTrue(four.IFoos.Count() == 4, "wrong number in collection");
        }


        [TestMethod]
        public void Bug149043()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<CFoo1>().Export<IFoo>((c) => c.Named("one"));
            ctx.ForType<CFoo2>().Export<IFoo>((c) => c.Named("two"));
            ctx.ForType<CFoo3>().Export<IFoo>((c) => c.Named("three"));
            ctx.ForType<CFoo4>().Export<IFoo>((c) => c.Named("four"));

            ctx.ForType<FooImporterBase>().ImportProperty<IFoo>((foo) => foo.IFoos, (c) => c.AsMany());
            ctx.ForType<FooImporter1>().SelectConstructor((ParameterImportBuilder p) => new FooImporter1(p.Import<IFoo>((b) => b.Named("one")))).Export();

            // ctx.ForType<FooImporterBase>().ImportProperty<IFoo>( (foo) => foo.IFoos, (c) => c.AsMany() );
            ctx.ForType<FooImporter2>()
                .SelectConstructor((ParameterImportBuilder p) => new FooImporter2(p.Import<IFoo>((b) => b.Named("one"))))
                .ImportProperty<IFoo>((foo) => foo.IFoos, (c) => c.AsMany()).Export();

            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(
                typeof(CFoo1), typeof(CFoo2), typeof(CFoo3), typeof(CFoo4), typeof(FooImporter1), typeof(FooImporter2)
                ), ctx);

            Assert.IsNotNull(catalog.Parts.Count() == 6, "fooImporter not set!");

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            var one = container.GetExportedValue<FooImporter1>();
            var two = container.GetExportedValue<FooImporter2>();

            Assert.IsNotNull(one.ImportedFoo != null, "one.fooImporter not set!");
            Assert.IsTrue(one.IFoos.Count() == 4, "one wrong number in collection");

            Assert.IsNotNull(two.ImportedFoo != null, "two.fooImporter not set!");
            Assert.IsTrue(two.IFoos.Count() == 4, "two wrong number in collection");
        }
    }
}
