// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;


namespace System.ComponentModel.Composition
{
    [TestClass]
    public class GenericsTests
    {
        public class Bar
        {
        }

        public class Bar2 : Bar
        {
        }

        public struct FooStruct
        { }


        public interface IFoo { }
        public interface IFoo2 : IFoo { }
        public interface IBar { }
        public interface IExport<T1, T2> { }
        public class ExportImpl<T1, T2> : IExport<T1, T2> {}

        public interface IExport<T> { }
        public interface IImport<T1, T2> { }
        public interface IImport<T> { }

        [Export(typeof(IFoo))]
        public class Foo : IFoo
        {
        }

        public interface IPartWithImport
        {
            object GetValue();
        }



        [Export(typeof(IImport<,>))]
        public class SelfImport<T1, T2> : IImport<T1, T2>
        {

        }

        [Export(typeof(IImport<>))]
        public class SelfImport<T> : IImport<T>
        {

        }

        [Export(typeof(IExport<,>))]
        public class SelfExport<T1, T2> : IExport<T1, T2>
        {
            
        }

        public class PropertyExport<T1, T2> : IExport<T1, T2>
        {
            [Export(typeof(IExport<,>))]
            IExport<T1, T2> Property { get { return this; } }
        }


        public class PropertyExportWithContractInferred<T1, T2> : IExport<T1, T2>
        {
            [Export]
            IExport<T1, T2> PropertyExport { get { return this; } }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithPropertyImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [Import(typeof(IImport<,>))]
            IImport<T1, T2> Value { get;  set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        public class PropertyExportWithChangedParameterOrder<T1, T2>
        {
            public PropertyExportWithChangedParameterOrder()
            {
                this.Export = new ExportImpl<T2, T1>();
            }

            [Export]
            public IExport<T2, T1> Export { get; set; }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithLazyPropertyImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [Import]
            Lazy<IImport<T1, T2>> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }


        [Export(typeof(IExport<>))]
        public class SelfExportWithNakedLazyPropertyImport<T> : IExport<T>, IPartWithImport
        {
            [Import]
            Lazy<T> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithExportFactoryPropertyImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [Import]
            ExportFactory<IImport<T1, T2>> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<>))]
        public class SelfExportWithNakedExportFactoryPropertyImport<T> : IExport<T>, IPartWithImport
        {
            [Import]
            ExportFactory<T> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }


        [Export(typeof(IExport<,>))]
        public class SelfExportWithExportFactoryParameterImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [ImportingConstructor]
            SelfExportWithExportFactoryParameterImport(ExportFactory<IImport<T1, T2>> value)
            {
                this.Value = value;
            }

            private ExportFactory<IImport<T1, T2>> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithCollectionPropertyImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [ImportMany]
            IEnumerable<IImport<T1, T2>> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithLazyCollectionPropertyImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [ImportMany]
            IEnumerable<Lazy<IImport<T1, T2>>> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithParameterImport<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [ImportingConstructor]
            SelfExportWithParameterImport(IImport<T1, T2> import)
            {
                this.Value = import;
            }

            IImport<T1, T2> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithPropertyImportWithContractInferred<T1, T2> : IExport<T1, T2>, IPartWithImport
        {
            [Import]
            IImport<T1, T2> Value { get; set; }

            public object GetValue()
            {
                return this.Value;
            }
        }

        [Export(typeof(IExport<,>))]
        public class SelfExportWithMultipleGenericImports<T1, T2> : IExport<T1, T2>
        {
            [Import]
            public IImport<T1> Import1 { get; set; }

            [Import]
            public IImport<T2> Import2 { get; set; }

            [Import]
            public IImport<T1, T2> Import3 { get; set; }

            [Import]
            public IImport<T2, T1> Import4 { get; set; }

            [Import]
            public IFoo Import5 { get; set; }

            [Import]
            public T1 Import6 { get; set; }

        }

        [Export(typeof(IExport<IFoo, IBar>))]
        public class ExportFooBar : IExport<IFoo, IBar>
        {
        }

        public static class SingletonExportExportCount
        {
            public static int Count { get; set; }
        }


        [Export(typeof(IExport<,>))]
        public class SingletonExport<T1, T2> : IExport<T1, T2>
        {
            public SingletonExport()
            {
                SingletonExportExportCount.Count++;
            }
        }

        public class SingletonImport<T1, T2>
        {
            [Import]
            public IExport<T1, T2> Import { get; set; }
        }

        [Export(typeof(IExport<>))]
        public class PartWithTypeConstraint<T> : IExport<T> where T : IFoo
        {
        }

        [Export(typeof(IExport<>))]
        public class PartWithBaseTypeConstraint<T> : IExport<T> where T : Bar
        {
        }


        [Export(typeof(IExport<>))]
        public class PartWithRefTypeConstraint<T> : IExport<T> where T : class
        {
        }


        [Export(typeof(IExport<>))]
        public class PartWithStructTypeConstraint<T> : IExport<T> where T : struct
        {
        }

        [Export(typeof(IExport<>))]
        public class PartWithNewableTypeConstraint<T> : IExport<T> where T : new()
        {
        }

        [Export(typeof(IExport<,>))]
        public class PartWithGenericConstraint<T1, T2> : IExport<T1, T2> where T2 : IDictionary<string, T1>
        {
        }

        [Export(typeof(IExport<,>))]
        public class PartWithNakedConstraint<T1, T2> : IExport<T1, T2> where T2 : T1
        {
        }

        [Export(typeof(IExport<>))]
        public class OpenGenericPartWithClosedGenericImport<T> : IExport<T>
        {
            [Import]
            public IImport<string> ClosedImport { get; set; }
        }

        [TestMethod]
        public void SelfExportWithClosedGenericImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfImport<>), typeof(OpenGenericPartWithClosedGenericImport<>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<object>>();
            Assert.IsNotNull(export);

            OpenGenericPartWithClosedGenericImport<object> impl = export as OpenGenericPartWithClosedGenericImport<object>;
            Assert.IsNotNull(impl);
            Assert.IsNotNull(impl.ClosedImport);
        }
       
        [TestMethod]
        public void SelfExportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var export2 = container.GetExportedValueOrDefault<IExport<IBar, IFoo>>();
            Assert.IsNotNull(export2);
        }

        [TestMethod]
        public void PropertyExportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PropertyExport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var export2 = container.GetExportedValueOrDefault<IExport<IBar, IFoo>>();
            Assert.IsNotNull(export2);
        }

        [TestMethod]
        public void PropertyExportWithContractInferredTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PropertyExportWithContractInferred<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var export2 = container.GetExportedValueOrDefault<IExport<IBar, IFoo>>();
            Assert.IsNotNull(export2);
        }

        [TestMethod]
        public void SelfExportWithPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithPropertyImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }

        [TestMethod]
        public void SelfExportWithLazyPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithLazyPropertyImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }

        [TestMethod]
        public void SelfExportWithNakedLazyPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithNakedLazyPropertyImport<>), typeof(Foo));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }

        [TestMethod]
        public void SelfExportWithExportFactoryPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithExportFactoryPropertyImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);

            var value = partWithImport.GetValue() as ExportFactory<IImport<IFoo, IBar>>;
            Assert.IsNotNull(value);

            using (var efv  = value.CreateExport())
            {
                 Assert.IsNotNull(efv.Value);
            }
        }

        [TestMethod]
        public void SelfExportWithNakedExportFactoryPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithNakedExportFactoryPropertyImport<>), typeof(Foo));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);

            var value = partWithImport.GetValue() as ExportFactory<IFoo>;
            Assert.IsNotNull(value);

            using (var efv = value.CreateExport())
            {
                Assert.IsNotNull(efv.Value);
            }
        }

        [TestMethod]
        public void SelfExportWithExportFactoryParameterImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithExportFactoryParameterImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);

            var value = partWithImport.GetValue() as ExportFactory<IImport<IFoo, IBar>>;
            Assert.IsNotNull(value);

            using (var efv  = value.CreateExport())
            {
                 Assert.IsNotNull(efv.Value);
            }
        }

        [TestMethod]
        public void SelfExportWithCollectionPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithCollectionPropertyImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }


        [TestMethod]
        public void SelfExportWithLazyCollectionPropertyImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithLazyCollectionPropertyImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }

        [TestMethod]
        public void SelfExportWithPropertyImportWithContractInferredTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithPropertyImportWithContractInferred<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }

        [TestMethod]
        public void SelfExportWithParameterImportTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithParameterImport<,>), typeof(SelfImport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var partWithImport = export as IPartWithImport;
            Assert.IsNotNull(partWithImport);
            Assert.IsNotNull(partWithImport.GetValue());
        }


        [TestMethod]
        public void SelfExportWithMultipleGenericImportsTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExportWithMultipleGenericImports<,>), typeof(SelfImport<,>), typeof(SelfImport<>), typeof(Foo));
            CompositionContainer container = new CompositionContainer(catalog);

            var export = container.GetExportedValueOrDefault<IExport<IFoo, IBar>>();
            Assert.IsNotNull(export);

            var part = export as SelfExportWithMultipleGenericImports<IFoo, IBar>;

            Assert.IsNotNull(part);
            Assert.IsNotNull(part.Import1);
            Assert.IsNotNull(part.Import2);
            Assert.IsNotNull(part.Import3);
            Assert.IsNotNull(part.Import4);
            Assert.IsNotNull(part.Import5);
            Assert.IsNotNull(part.Import6);
        }

        [TestMethod]
        public void SpecilzationMakesGeneric()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SelfExport<,>), typeof(ExportFooBar), typeof(SelfExport<IFoo, IBar>));
            CompositionContainer container = new CompositionContainer(catalog);

            // we are expecting 3 - one from the open generic, one from the closed generic and one from the specialization
            var exports = container.GetExportedValues<IExport<IFoo, IBar>>().ToArray();
            Assert.AreEqual(3, exports.Length);
        }

        [TestMethod]
        public void SingletonBehavior()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(SingletonExport<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            SingletonExportExportCount.Count = 0;

            var exports = container.GetExportedValues<IExport<IFoo, IBar>>();
            Assert.AreEqual(1, exports.Count());
            // only one instance of the SingletonExport<,> is created
            Assert.AreEqual(1, SingletonExportExportCount.Count);


            exports = container.GetExportedValues<IExport<IFoo, IBar>>();
            Assert.AreEqual(1, exports.Count());
            // still only one instance of the SingletonExport<,> is created
            Assert.AreEqual(1, SingletonExportExportCount.Count);

            var import = new SingletonImport<IFoo, IBar>();
            container.SatisfyImportsOnce(import);
            // still only one instance of the SingletonExport<,> is created
            Assert.AreEqual(1, SingletonExportExportCount.Count);

            import = new SingletonImport<IFoo, IBar>();
            container.SatisfyImportsOnce(import);
            // still only one instance of the SingletonExport<,> is created
            Assert.AreEqual(1, SingletonExportExportCount.Count);


            var import2 = new SingletonImport<IBar, IFoo>();
            container.SatisfyImportsOnce(import2);
            // two instances of the SingletonExport<,> is created
            Assert.AreEqual(2, SingletonExportExportCount.Count);
        }

        [TestMethod]
        public void PartWithTypeConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithTypeConstraint<>));
            CompositionContainer container = new CompositionContainer(catalog);

            // IFoo should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<IFoo>>().Count());

            // IFoo2 should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<IFoo2>>().Count());

            // IBar shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<IBar>>().Count());
        }

        [TestMethod]
        public void PartWithBaseTypeConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithBaseTypeConstraint<>));
            CompositionContainer container = new CompositionContainer(catalog);

            // bar should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<Bar>>().Count());

            // bar2 should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<Bar2>>().Count());

            // IFoo shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<IFoo>>().Count());
        }

        [TestMethod]
        public void PartWithRefTypeConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithRefTypeConstraint<>));
            CompositionContainer container = new CompositionContainer(catalog);

            // IFoo should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<IFoo>>().Count());

            // Bar should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<Bar>>().Count());

            // int shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<int>>().Count());


            // FooStruct shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<FooStruct>>().Count());
        }

        [TestMethod]
        public void PartWithStructTypeConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithStructTypeConstraint<>));
            CompositionContainer container = new CompositionContainer(catalog);

            // int should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<int>>().Count());

            // FooStruct should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<FooStruct>>().Count());

            // IFoo shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<IFoo>>().Count());

            // Bar shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<Bar>>().Count());

        }

        [TestMethod]
        public void PartWithNewableTypeConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithNewableTypeConstraint<>));
            CompositionContainer container = new CompositionContainer(catalog);

            // int should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<int>>().Count());

            // FooStruct should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<FooStruct>>().Count());

            // IFoo shouldn't
            Assert.AreEqual(0, container.GetExportedValues<IExport<IFoo>>().Count());

            // Bar should
            Assert.AreEqual(1, container.GetExportedValues<IExport<Bar>>().Count());

        }

        [TestMethod]
        public void PartWithGenericConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithGenericConstraint<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            // int, Dictionary<string, int> should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<int, Dictionary<string, int>>>().Count());

            // int, Dictionary<string, string> should not work
            Assert.AreEqual(0, container.GetExportedValues<IExport<int, Dictionary<string, string>>>().Count());

            // FooStruct, FooStruct[] should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<FooStruct, Dictionary<string, FooStruct>>>().Count());

            // FooStruct, IFoo should not
            Assert.AreEqual(0, container.GetExportedValues<IExport<FooStruct, IFoo>>().Count());

        }

        [TestMethod]
        public void PartWithNakedConstraintTest()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PartWithNakedConstraint<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            // Bar, Bar2 should work
            Assert.AreEqual(1, container.GetExportedValues<IExport<Bar, Bar2>>().Count());

            // Bar2, Bar should not work
            Assert.AreEqual(0, container.GetExportedValues<IExport<Bar2, Bar>>().Count());
        }

        [TestMethod]
        public void PartWithExportParametersInReverseOrder()
        {
            TypeCatalog catalog = new TypeCatalog(typeof(PropertyExportWithChangedParameterOrder<,>));
            CompositionContainer container = new CompositionContainer(catalog);

            Assert.AreEqual(1, container.GetExportedValues<IExport<string, int>>().Count());
        }

    }
}
