// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionServiceExportFactoryTests
    {
        public interface IFoo 
        {
            void DoWork();
            Child FooChild { get; set; }
        }
    
        [Export(typeof(IFoo))]
        class Foo1 : IFoo
        {
            public void DoWork()
            {
                Console.WriteLine("HelloWorld : {0}", FooChild.FooValue);
            }
    
            [Import("FooChild")]
            public Child FooChild { get; set; }
        }
    
        [Export("FooChild")]
        public class Child
        {
            public int FooValue { get; set; }
        }
    
        [Export]
        public class App
        {
            [Import]
            public ExportFactory<IFoo> FooFactory { get; set; }
        }

        [TestMethod]
        [Description("Verifies CompositionServices.SatisfyImportsOne with Scoped ExportFactories")]
        public void ComposeAppInNewScopeChildrenInRoot_ShouldSucceed()
        {
            var childCatalog = new CompositionScopeDefinition(new TypeCatalog(typeof(Foo1)), new CompositionScopeDefinition[] { });
            var rootCatalog = new CompositionScopeDefinition(new TypeCatalog(typeof(Child)), new CompositionScopeDefinition[] { childCatalog });

            var cs = rootCatalog.CreateCompositionService();
            var app = new App();

            cs.SatisfyImportsOnce(app);

            var e1 = app.FooFactory.CreateExport();
            var e2 = app.FooFactory.CreateExport();
            var e3 = app.FooFactory.CreateExport();
            e1.Value.FooChild.FooValue = 10;
            e2.Value.FooChild.FooValue = 20;
            e3.Value.FooChild.FooValue = 30;

            Assert.AreEqual<int>(e1.Value.FooChild.FooValue, 30);
            Assert.AreEqual<int>(e2.Value.FooChild.FooValue, 30);
            Assert.AreEqual<int>(e3.Value.FooChild.FooValue, 30);
        }

        [TestMethod]
        [Description("Verifies CompositionServices.SatisfyImportsOne with Scoped ExportFactories")]
        public void ComposeAppInNewScopeChildrenInScope_ShouldSucceed()
        {
            var childCatalog = new CompositionScopeDefinition(new TypeCatalog(typeof(Foo1), typeof(Child)), new CompositionScopeDefinition[] { });
            var rootCatalog = new CompositionScopeDefinition(new TypeCatalog(), new CompositionScopeDefinition[] { childCatalog });

            var cs = rootCatalog.CreateCompositionService();
            var app = new App();

            cs.SatisfyImportsOnce(app);

            var e1 = app.FooFactory.CreateExport();
            var e2 = app.FooFactory.CreateExport();
            var e3 = app.FooFactory.CreateExport();
            e1.Value.FooChild.FooValue = 10;
            e2.Value.FooChild.FooValue = 20;
            e3.Value.FooChild.FooValue = 30;

            Assert.AreEqual<int>(e1.Value.FooChild.FooValue, 10);
            Assert.AreEqual<int>(e2.Value.FooChild.FooValue, 20);
            Assert.AreEqual<int>(e3.Value.FooChild.FooValue, 30);
        }

        [TestMethod]
        [Description("Verifies CompositionServices.SatisfyImportsOne with Scoped ExportFactories")]
        public void ComposeAppInNewScopeChildrenInBoth_ShouldSucceed()
        {
            var childCatalog = new CompositionScopeDefinition(new TypeCatalog(typeof(Foo1), typeof(Child)), new CompositionScopeDefinition[] { });
            var rootCatalog = new CompositionScopeDefinition(new TypeCatalog(typeof(Child)), new CompositionScopeDefinition[] { childCatalog });

            var cs = rootCatalog.CreateCompositionService();
            var app = new App();

            cs.SatisfyImportsOnce(app);

            var e1 = app.FooFactory.CreateExport();
            var e2 = app.FooFactory.CreateExport();
            var e3 = app.FooFactory.CreateExport();
            e1.Value.FooChild.FooValue = 10;
            e2.Value.FooChild.FooValue = 20;
            e3.Value.FooChild.FooValue = 30;

            Assert.AreEqual<int>(e1.Value.FooChild.FooValue, 10);
            Assert.AreEqual<int>(e2.Value.FooChild.FooValue, 20);
            Assert.AreEqual<int>(e3.Value.FooChild.FooValue, 30);
        }

        [TestMethod]
        [Description("Verifies CompositionServices.SatisfyImportsOne with NonScoped ExportFactories")]
        public void ComposeAppInRootScope_ShouldSucceed()
        {
            var catalog = new TypeCatalog(typeof(Foo1), typeof(Child));
 
            var cs = catalog.CreateCompositionService();
            var app = new App();

            cs.SatisfyImportsOnce(app);

            var e1 = app.FooFactory.CreateExport();
            var e2 = app.FooFactory.CreateExport();
            var e3 = app.FooFactory.CreateExport();
            e1.Value.FooChild.FooValue = 10;
            e2.Value.FooChild.FooValue = 20;
            e3.Value.FooChild.FooValue = 30;

            Assert.AreEqual<int>(e1.Value.FooChild.FooValue, 30);
            Assert.AreEqual<int>(e2.Value.FooChild.FooValue, 30);
            Assert.AreEqual<int>(e3.Value.FooChild.FooValue, 30);
        }
    }
}
