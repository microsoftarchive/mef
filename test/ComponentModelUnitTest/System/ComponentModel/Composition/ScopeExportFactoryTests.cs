// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ScopeExportFactoryTests
    {
        public interface IFooContract
        {
        }

        public interface IFooMetadata
        {
            string Name { get; }
        }

        public interface IBarContract
        {
            IFooContract CreateFoo();
        }


        public interface IBlahContract
        {
        }

        [Export(typeof(IFooContract))]
        [ExportMetadata("Name", "Foo")]
        public class FooImpl : IFooContract
        {
        }

        [Export(typeof(IFooContract))]
        [ExportMetadata("Name", "Foo")]
        public class Foo2Impl : IFooContract
        {
        }

        [Export(typeof(IFooContract))]
        public class Foo3Impl : IFooContract
        {
            [Import]
            public IBlahContract Blah { get; set; }
        }

        [Export(typeof(IFooContract))]
        public class Foo4Impl : IFooContract
        {
            [Import]
            public ExportFactory<IFooContract> Blah { get; set; }
        }

        [Export(typeof(IBlahContract))]
        public class BlahImpl : IBlahContract
        {
            [Import]
            public IBlahContract Blah { get; set; }
        }

        [Export(typeof(IBarContract))]
        public class BarImpl : IBarContract
        {
            [Import]
            public ExportFactory<IFooContract> FooFactory { get; set; }

            public IFooContract CreateFoo()
            {
                var efv = this.FooFactory.CreateExport();
                var value = efv.Value;
                efv.Dispose();
                return value;
            }
        }

        [Export(typeof(IBarContract))]
        public class BarWithMany : IBarContract
        {
            [ImportMany]
            public ExportFactory<IFooContract>[] FooFactories { get; set; }

            public IFooContract CreateFoo()
            {
                var efv = this.FooFactories[0].CreateExport();
                var value = efv.Value;
                efv.Dispose();
                return value;
            }
        }

        [Export(typeof(IBarContract))]
        public class BarImplWithMetadata : IBarContract
        {
            [Import]
            public ExportFactory<IFooContract, IFooMetadata> FooFactory { get; set; }

            public IFooContract CreateFoo()
            {
                Assert.AreEqual("Foo", this.FooFactory.Metadata.Name);
                var efv = this.FooFactory.CreateExport();
                var value = efv.Value;
                efv.Dispose();
                return value;
            }
        }

        [TestMethod]
        public void SimpleChain()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(FooImpl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValue<IBarContract>();
            Assert.IsNotNull(bar);

            var foo1 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo1 is FooImpl);

            var foo2 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo2 is FooImpl);

            Assert.AreNotEqual(foo1, foo2);
        }


        [TestMethod]
        public void SimpleChainWithTwoChildren()
        {
            var parentCatalog = new TypeCatalog(typeof(BarWithMany));
            var childCatalog1 = new TypeCatalog(typeof(FooImpl));
            var childCatalog2 = new TypeCatalog(typeof(Foo2Impl));

            var scope = parentCatalog.AsScope(childCatalog1.AsScope(), childCatalog2.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValue<IBarContract>() as BarWithMany;
            Assert.IsNotNull(bar);

            Assert.AreEqual(2, bar.FooFactories.Length);

            IFooContract foo1 = null;
            using (var efFoo1 = bar.FooFactories[0].CreateExport())
            {
                foo1 = efFoo1.Value;
            }

            IFooContract foo2 = null;
            using (var efFoo2 = bar.FooFactories[1].CreateExport())
            {
                foo2 = efFoo2.Value;
            }

            Assert.IsTrue(((foo1 is FooImpl) && (foo2 is Foo2Impl)) || ((foo2 is FooImpl) && (foo1 is Foo2Impl)));
        }

        [TestMethod]
        public void SimpleChainWithMetadata()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImplWithMetadata));
            var childCatalog = new TypeCatalog(typeof(FooImpl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValue<IBarContract>();
            Assert.IsNotNull(bar);

            var foo1 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo1 is FooImpl);

            var foo2 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo2 is FooImpl);

            Assert.AreNotEqual(foo1, foo2);
        }

        [TestMethod]
        public void SimpleChainWithLowerLoop()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(Foo3Impl), typeof(BlahImpl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValue<IBarContract>();
            Assert.IsNotNull(bar);

            var foo1 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo1 is Foo3Impl);

            var foo2 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo2 is Foo3Impl);

            Assert.AreNotEqual(foo1, foo2);
        }

        [TestMethod]
        public void SimpleChainWithCrossLoop()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(Foo4Impl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValue<IBarContract>();
            Assert.IsNotNull(bar);

            var foo1 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo1 is Foo4Impl);

            var foo2 = bar.CreateFoo();
            Assert.IsNotNull(foo1);
            Assert.IsTrue(foo2 is Foo4Impl);

            Assert.AreNotEqual(foo1, foo2);
        }

        [TestMethod]
        public void SimpleChainWithLowerLoopRejection()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(Foo3Impl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValueOrDefault<IBarContract>();
            Assert.IsNull(bar);
        }

        [TestMethod]
        public void ExportFactoryCausesRejectionBasedOnContract()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(BarImpl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValueOrDefault<IBarContract>();
            Assert.IsNull(bar);
        }

        [TestMethod]
        public void ExportFactoryCausesRejectionBasedOnCardinality()
        {
            var parentCatalog = new TypeCatalog(typeof(BarImpl));
            var childCatalog = new TypeCatalog(typeof(FooImpl), typeof(Foo2Impl));

            var scope = parentCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            var bar = container.GetExportedValueOrDefault<IBarContract>();
            Assert.IsNull(bar);
        }
    }

    [TestClass]
    public class FilteredScopeFactoryOfT
    {
        public class FilteredExportFactory<T> : ExportFactory<T>
        {
            public FilteredExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator) : base(exportLifetimeContextCreator)
            {
            }

            protected override bool OnFilterScopedCatalog(ComposablePartDefinition composablePartDefinition)
            {
                return composablePartDefinition.ContainsPartMetadata("filter", ValueToFilter);
            }

            public string ValueToFilter { get; set; }
        }

        
        public interface IFiltered {}

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassB : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassC : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInSubsequentInstances")]
        public class ClassD : IFiltered
        {
        }

        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public FilteredExportFactory<ClassA> classA { get; set; }

            [ImportAttribute] 
            public ClassA localClassA { get; set; }
        }

        [Export]
        public class ClassA
        {
            [ImportMany]
            public IEnumerable<IFiltered> filteredTypes  { get; set; }

            public int InstanceValue  { get; set; }
        }

        [TestMethod]
        public void FilteredScopeFactoryOfT_ShouldSucceed()
        {
            var c1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA) );
            var c2 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD) );
            var sd = c1.AsScope( c2.AsScope() );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            fromRoot.classA.ValueToFilter ="IncludeInFirstInstance";
            var a1 = fromRoot.classA.CreateExport().Value;

            fromRoot.classA.ValueToFilter ="IncludeInSubsequentInstances";
            var a2 = fromRoot.classA.CreateExport().Value;

            Assert.AreEqual(fromRoot.localClassA.filteredTypes.Count(), 0);
            Assert.AreEqual(a1.filteredTypes.Count(), 2);
            Assert.AreEqual(a2.filteredTypes.Count(), 1);
        }
    }

    [TestClass]
    public class FilteredScopeFactoryOfTM
    {
        public class FilteredExportFactory<T, M> : ExportFactory<T, M>
        {
            public FilteredExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator, M metadata) : base(exportLifetimeContextCreator, metadata)
            {
            }

            protected override bool OnFilterScopedCatalog(ComposablePartDefinition composablePartDefinition)
            {
                return composablePartDefinition.ContainsPartMetadata("filter", ValueToFilter);
            }

            public string ValueToFilter { get; set; }
        }

        public interface IMetadataView
        {
            string DocType { get;  }
        }

        public interface IFiltered {}

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassB : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassC : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInSubsequentInstances")]
        public class ClassD : IFiltered
        {
        }

        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public FilteredExportFactory<ClassA, IMetadataView> classA;

            [ImportAttribute] 
            public ClassA localClassA;
        }

        [ExportMetadata("DocType", "ClassA")]
        [Export]
        public class ClassA
        {
            [ImportMany]
            public IEnumerable<IFiltered> filteredTypes;

            public int InstanceValue;
        }

        [TestMethod]
        public void FilteredScopeFactoryOfTM_ShouldSucceed()
        {
            var c1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA) );
            var c2 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD) );
            var sd = c1.AsScope( c2.AsScope() );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            fromRoot.classA.ValueToFilter ="IncludeInFirstInstance";
            var a1 = fromRoot.classA.CreateExport().Value;
            var md = fromRoot.classA.Metadata;

            fromRoot.classA.ValueToFilter ="IncludeInSubsequentInstances";
            var a2 = fromRoot.classA.CreateExport().Value;

            Assert.AreEqual(fromRoot.localClassA.filteredTypes.Count(), 0);
            Assert.AreEqual(a1.filteredTypes.Count(), 2);
            Assert.AreEqual(a2.filteredTypes.Count(), 1);
        }
    }

    public class FilteredScopeFactoryWithTooManyGenericArguments
    {
        public class FilteredExportFactory<T, M, V> : ExportFactory<T, M>
        {
            public FilteredExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator, M metadata) : base(exportLifetimeContextCreator, metadata)
            {
            }

            protected override bool OnFilterScopedCatalog(ComposablePartDefinition composablePartDefinition)
            {
                return composablePartDefinition.ContainsPartMetadata("filter", ValueToFilter);
            }

            public V ValueToFilter { get; set; }
        }

        public interface IMetadataView
        {
            string DocType { get;  }
        }

        public interface IFiltered {}

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassB : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInFirstInstance")]
        public class ClassC : IFiltered
        {
        }

        [Export(typeof(IFiltered))]
        [PartMetadata("filter", "IncludeInSubsequentInstances")]
        public class ClassD : IFiltered
        {
        }

        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public FilteredExportFactory<ClassA, IMetadataView, string> classA;

            [ImportAttribute] 
            public ClassA localClassA;
        }

        [ExportMetadata("DocType", "ClassA")]
        [Export]
        public class ClassA
        {
            [ImportMany]
            public IEnumerable<IFiltered> filteredTypes;

            public int InstanceValue;
        }        
    }

    [TestClass]
    public class ScopeExportFactoryWithPublicSurface
    {
        [Export] public class ClassA {}
        [Export] public class ClassB {}
        [Export] public class ClassC {}

        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassA> classA;

            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassB> classB;

            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassB> classC;
        }

        [TestMethod]
        public void FilteredScopeFactoryOfTM_ShouldSucceed()
        {
            var c1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA) );
            var c2 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC) );
            var c3 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC) );
            var c4 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC) );
            var sd = c1.AsScope(c2.AsScopeWithPublicSurface<ClassA>(),
                                c3.AsScopeWithPublicSurface<ClassB>(),
                                c4.AsScopeWithPublicSurface<ClassC>() );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a = fromRoot.classA.CreateExport().Value;
            var b = fromRoot.classB.CreateExport().Value;
            var c = fromRoot.classC.CreateExport().Value;

        }
    }

    [TestClass]
    public class ScopeFactoryAutoResolveFromAncestorScope
    {
        [Export]public class Root {}
        [Export]public class Child {}

        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassA> classA;

            [ImportAttribute] 
            public ClassA localClassA;
        }

        [Export]
        public class ClassA
        {
            [Import]
            public ICompositionService CompositionService;

            [ImportAttribute] 
            public Root classRoot;

            public int InstanceValue;
        }

        public class ImportA
        {
            [Import]
            public ClassA classA;
        }

        [TestMethod]
        public void ScopeFactoryAutoResolveFromAncestorScopeShouldSucceed()
        {
            var c1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA), typeof(Root) );
            var c2 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA), typeof(Child) );
            var sd = c1.AsScope( c2.AsScope() );

            var container = new CompositionContainer(sd, CompositionOptions.ExportCompositionService);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a1 = fromRoot.classA.CreateExport().Value;
            var a2 = fromRoot.classA.CreateExport().Value;
            fromRoot.localClassA.InstanceValue = 101;
            a1.InstanceValue = 202;
            a2.InstanceValue = 303;

            Assert.AreNotEqual(a1.InstanceValue, a2.InstanceValue);
            Assert.IsNotNull(fromRoot.localClassA.classRoot);
            Assert.IsNotNull(a1.classRoot);
            Assert.IsNotNull(a2.classRoot);
        }
    }

    [TestClass]
    public class DeeplyNestedCatalog
    {
        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassA> classA;
        }

        [Export]
        public class ClassA
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassB> classB;
        }

        [Export]
        public class ClassB
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassC> classC;

            public int InstanceValue;
        }

        [Export]
        public class ClassC
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassD> classD;
        }

        [Export]
        public class ClassD
        {
        }

        [TestMethod]
        public void DeeplyNestedCatalogPartitionedCatalog_ShouldWork()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot));
            var cat2 = new TypeCatalog( typeof(ClassA) );
            var cat3 = new TypeCatalog( typeof(ClassB) );
            var cat4 = new TypeCatalog( typeof(ClassC) );
            var cat5 = new TypeCatalog( typeof(ClassD) );
            var sd = cat1.AsScope( cat2.AsScope( cat3.AsScope( cat4.AsScope( cat5.AsScope() ) ) ) );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();

            var a1 = fromRoot.classA.CreateExport().Value;
            var b1 = a1.classB.CreateExport().Value;
            var c1 = b1.classC.CreateExport().Value;
            var d1 = c1.classD.CreateExport().Value;
        }

        [TestMethod]
        public void DeeplyNestedCatalogOverlappedCatalog_ShouldWork()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD));
            var cat2 = cat1;
            var cat3 = cat1;
            var cat4 = cat1;
            var cat5 = cat1;
            var sd = cat1.AsScope( cat2.AsScope( cat3.AsScope( cat4.AsScope( cat5.AsScope() ) ) ) );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();

            var a1 = fromRoot.classA.CreateExport().Value;
            var b1 = a1.classB.CreateExport().Value;
            var c1 = b1.classC.CreateExport().Value;
            var d1 = c1.classD.CreateExport().Value;
        }
    }

    [TestClass]
    public class LocalSharedNonLocalInSameContainer
    {
        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassA> classA;

            [ImportAttribute] 
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassA
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassB> classB;

            [ImportAttribute] 
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassB
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassC> classC;

            [ImportAttribute]
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassC
        {
            [ImportAttribute(Source = ImportSource.NonLocal)] 
            public ClassXXXX xxxx;

            [Import]
            public ClassD classD;
        }

        [Export]
        public class ClassD
        {
            [ImportAttribute] 
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassXXXX
        {
            public int InstanceValue;
        }

        [TestMethod]
        public void LocalSharedNonLocalInSameContainer_ShouldSucceed()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassXXXX) );
            var cat2 = new TypeCatalog( typeof(ClassA) );
            var cat3 = new TypeCatalog( typeof(ClassB) );
            var cat4 = new TypeCatalog( typeof(ClassC), typeof(ClassD), typeof(ClassXXXX) );
            var sd = cat1.AsScope( cat2.AsScope( cat3.AsScope( cat4.AsScope( ) )) );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a1 = fromRoot.classA.CreateExport().Value;
            fromRoot.xxxx.InstanceValue = 16;
            var b1 = a1.classB.CreateExport().Value;
            var c1 = b1.classC.CreateExport().Value;

            Assert.AreEqual(16, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(16, a1.xxxx.InstanceValue);
            Assert.AreEqual(16, b1.xxxx.InstanceValue);
            Assert.AreEqual(16, c1.xxxx.InstanceValue);
            Assert.AreEqual(0,  c1.classD.xxxx.InstanceValue);

            c1.xxxx.InstanceValue = 8;

            Assert.AreEqual(8, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(8, a1.xxxx.InstanceValue);
            Assert.AreEqual(8, b1.xxxx.InstanceValue);
            Assert.AreEqual(8, c1.xxxx.InstanceValue);
            Assert.AreEqual(0, c1.classD.xxxx.InstanceValue);

            c1.classD.xxxx.InstanceValue = 2;
            Assert.AreEqual(8, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(8, a1.xxxx.InstanceValue);
            Assert.AreEqual(8, b1.xxxx.InstanceValue);
            Assert.AreEqual(8, c1.xxxx.InstanceValue);
            Assert.AreEqual(2, c1.classD.xxxx.InstanceValue);

        }
    }


    [TestClass]
    public class ScopeBridgingAdaptersConstructorInjection
    {
        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassC> classC;

            [ImportAttribute] 
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassC
        {
            [ImportingConstructor]
            public ClassC([ImportAttribute(RequiredCreationPolicy = CreationPolicy.NonShared, Source = ImportSource.NonLocal)]ClassXXXX xxxx)
            {
                this.xxxx = xxxx;
            }

            [Export]
            public ClassXXXX xxxx;

            [Import]
            public ClassD classD;
        }


        [Export]
        public class ClassD
        {
            [Import]
            public ClassXXXX xxxx;
        }


        [Export]
        public class ClassXXXX
        {
            public int InstanceValue;
        }

        [TestMethod]
        public void ScopeBridgingAdapters_ShouldSucceed()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassXXXX) );
            var cat2 = new TypeCatalog( typeof(ClassC), typeof(ClassD) );
            var sd = cat1.AsScope(cat2.AsScope());
            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var c1 = fromRoot.classC.CreateExport().Value;
            var c2 = fromRoot.classC.CreateExport().Value;
            var c3 = fromRoot.classC.CreateExport().Value;
            var c4 = fromRoot.classC.CreateExport().Value;
            var c5 = fromRoot.classC.CreateExport().Value;

            Assert.AreEqual(0, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(0, c1.xxxx.InstanceValue);
            Assert.AreEqual(0, c1.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c2.xxxx.InstanceValue);
            Assert.AreEqual(0, c2.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c3.xxxx.InstanceValue);
            Assert.AreEqual(0, c3.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c4.xxxx.InstanceValue);
            Assert.AreEqual(0, c4.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c5.xxxx.InstanceValue);
            Assert.AreEqual(0, c5.classD.xxxx.InstanceValue);

            c1.xxxx.InstanceValue = 1;
            c2.xxxx.InstanceValue = 2;
            c3.xxxx.InstanceValue = 3;
            c4.xxxx.InstanceValue = 4;
            c5.xxxx.InstanceValue = 5;

            Assert.AreEqual(0, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(1, c1.xxxx.InstanceValue);
            Assert.AreEqual(1, c1.classD.xxxx.InstanceValue);
            Assert.AreEqual(2, c2.xxxx.InstanceValue);
            Assert.AreEqual(2, c2.classD.xxxx.InstanceValue);
            Assert.AreEqual(3, c3.xxxx.InstanceValue);
            Assert.AreEqual(3, c3.classD.xxxx.InstanceValue);
            Assert.AreEqual(4, c4.xxxx.InstanceValue);
            Assert.AreEqual(4, c4.classD.xxxx.InstanceValue);
            Assert.AreEqual(5, c5.xxxx.InstanceValue);
            Assert.AreEqual(5, c5.classD.xxxx.InstanceValue);
        }
    }


    [TestClass]
    public class ScopeBridgingAdaptersImportExportProperty
    {
        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassC> classC;

            [ImportAttribute] 
            public ClassXXXX xxxx;
        }

        [Export]
        public class ClassC
        {
            [Export][ImportAttribute(RequiredCreationPolicy = CreationPolicy.NonShared, Source = ImportSource.NonLocal)]
            public ClassXXXX xxxx;

            [Import]
            public ClassD classD;
        }


        [Export]
        public class ClassD
        {
            [Import]
            public ClassXXXX xxxx;
        }


        [Export]
        public class ClassXXXX
        {
            public int InstanceValue;
        }

        [TestMethod]
        public void ScopeBridgingAdaptersImportExportProperty_ShouldSucceed()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassXXXX) );
            var cat2 = new TypeCatalog( typeof(ClassC), typeof(ClassD) );
            var sd = cat1.AsScope(cat2.AsScope());
            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var c1 = fromRoot.classC.CreateExport().Value;
            var c2 = fromRoot.classC.CreateExport().Value;
            var c3 = fromRoot.classC.CreateExport().Value;
            var c4 = fromRoot.classC.CreateExport().Value;
            var c5 = fromRoot.classC.CreateExport().Value;

            Assert.AreEqual(0, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(0, c1.xxxx.InstanceValue);
            Assert.AreEqual(0, c1.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c2.xxxx.InstanceValue);
            Assert.AreEqual(0, c2.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c3.xxxx.InstanceValue);
            Assert.AreEqual(0, c3.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c4.xxxx.InstanceValue);
            Assert.AreEqual(0, c4.classD.xxxx.InstanceValue);
            Assert.AreEqual(0, c5.xxxx.InstanceValue);
            Assert.AreEqual(0, c5.classD.xxxx.InstanceValue);

            c1.xxxx.InstanceValue = 1;
            c2.xxxx.InstanceValue = 2;
            c3.xxxx.InstanceValue = 3;
            c4.xxxx.InstanceValue = 4;
            c5.xxxx.InstanceValue = 5;

            Assert.AreEqual(0, fromRoot.xxxx.InstanceValue);
            Assert.AreEqual(1, c1.xxxx.InstanceValue);
            Assert.AreEqual(1, c1.classD.xxxx.InstanceValue);
            Assert.AreEqual(2, c2.xxxx.InstanceValue);
            Assert.AreEqual(2, c2.classD.xxxx.InstanceValue);
            Assert.AreEqual(3, c3.xxxx.InstanceValue);
            Assert.AreEqual(3, c3.classD.xxxx.InstanceValue);
            Assert.AreEqual(4, c4.xxxx.InstanceValue);
            Assert.AreEqual(4, c4.classD.xxxx.InstanceValue);
            Assert.AreEqual(5, c5.xxxx.InstanceValue);
            Assert.AreEqual(5, c5.classD.xxxx.InstanceValue);
        }
    }

    [TestClass]
    public class SelfExportFromExportFactory
    {
        [Export]
        public class ClassRoot
        {
            [ImportAttribute(RequiredCreationPolicy = CreationPolicy.NewScope)] 
            public ExportFactory<ClassA> classA;
        }

        [Export]
        public class ClassA
        {
            [ImportAttribute]  public ClassB classB;
            [ImportAttribute]  public ClassC classC;
            [ImportAttribute]  public ClassD classD;

            public int InstanceValue;
        }

        [Export]
        public class ClassB
        {
            [ImportAttribute]  public ClassA classA;
        }

        [Export]
        public class ClassC
        {
            [ImportAttribute]  public ClassA classA;
        }

        [Export]
        public class ClassD
        {
            [ImportAttribute]  public ClassA classA;
        }

        [TestMethod]
        public void SelfExportFromExportFactory_ShouldSucceed()
        {
            var cat1 = new TypeCatalog( typeof(ClassRoot) );
            var cat2 = new TypeCatalog( typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD) );
            var sd = cat1.AsScope( cat2.AsScope() );

            var container = new CompositionContainer(sd);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a1 = fromRoot.classA.CreateExport().Value;

            a1.InstanceValue = 8;

            Assert.AreEqual(8, a1.classB.classA.InstanceValue);
            Assert.AreEqual(8, a1.classC.classA.InstanceValue);
            Assert.AreEqual(8, a1.classD.classA.InstanceValue);
        }
    }
}
