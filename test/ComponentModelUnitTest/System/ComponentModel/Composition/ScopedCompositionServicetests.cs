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


namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ScopedCompositionServiceTests
    {
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
            public int InstanceValue;
        }

        public class ImportA
        {
            [Import]
            public ClassA classA;
        }

        [Export]
        public class FromRoot
        {
            [Import]
            public ExportFactory<ClassRequiresICompositionService> Required { get; set; }   
            
            [Import]
            public ExportFactory<ClassOptionallyImportsICompositionService> Optional { get; set; }   
        }

        [Export]
        public class ClassRequiresICompositionService
        {
            [Import(AllowDefault=false)]
            public ICompositionService CompositionService { get; set;  }
        }

        [Export]
        public class ClassOptionallyImportsICompositionService
        {
            [Import(AllowDefault=true)]
            public ICompositionService CompositionService { get; set;  }
        }

        [TestMethod]
        public void DontExportICompositionServiceFromRootRequiredImportShouldThrowCompositionException()
        {
            var rootCatalog = new TypeCatalog(typeof(ClassRequiresICompositionService), typeof(ClassOptionallyImportsICompositionService));
            var container = new CompositionContainer(rootCatalog);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                var service = container.GetExportedValue<ClassRequiresICompositionService>();
                Assert.IsNull(service.CompositionService);
            });
        }

        [TestMethod]
        public void DontExportICompositionServiceFromRootOptionalImportShouldSucceed()
        {
            var rootCatalog = new TypeCatalog(typeof(ClassRequiresICompositionService),typeof(ClassOptionallyImportsICompositionService));
            var container = new CompositionContainer(rootCatalog);

            var  service = container.GetExportedValue<ClassOptionallyImportsICompositionService>();
            Assert.IsNull(service.CompositionService);
        }

        [TestMethod]
        public void ExportICompositionServiceFromRootRequiredImportShouldsucceed()
        {
            var rootCatalog = new TypeCatalog(typeof(ClassRequiresICompositionService),typeof(ClassOptionallyImportsICompositionService));
            var container = new CompositionContainer(rootCatalog, CompositionOptions.ExportCompositionService);

            var service = container.GetExportedValue<ClassRequiresICompositionService>();
            Assert.IsNotNull(service.CompositionService);
        }

        [TestMethod]
        public void ExportICompositionServiceFromRootOptionalImportShouldSucceed()
        {
            var rootCatalog = new TypeCatalog(typeof(ClassRequiresICompositionService),typeof(ClassOptionallyImportsICompositionService));
            var container = new CompositionContainer(rootCatalog, CompositionOptions.ExportCompositionService);

            var service = container.GetExportedValue<ClassOptionallyImportsICompositionService>();
            Assert.IsNotNull(service.CompositionService);
        }

        [TestMethod]
        public void DontExportICompositionServiceFromChildImportShouldShouldThrowCompositionException()
        {
            var rootCatalog = new TypeCatalog( typeof(FromRoot) );
            var childCatalog = new TypeCatalog( typeof(ClassRequiresICompositionService), typeof(ClassOptionallyImportsICompositionService) );
            var scope = rootCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope);

            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                var fromRoot = container.GetExportedValue<FromRoot>();
                Assert.IsNull(fromRoot);
            });
        }

        [TestMethod]
        public void ExportICompositionServiceFromChildImportShouldShouldSucceed()
        {
            var childCatalog = new TypeCatalog( typeof(ClassRequiresICompositionService), typeof(ClassOptionallyImportsICompositionService) );
            var rootCatalog = new TypeCatalog( typeof(FromRoot) );
            var scope = rootCatalog.AsScope(childCatalog.AsScope());
            var container = new CompositionContainer(scope, CompositionOptions.ExportCompositionService);

            var fromRoot = container.GetExportedValue<FromRoot>();

            var requiredService = fromRoot.Required.CreateExport();
            Console.WriteLine("requiredService: {0}", requiredService.Value.CompositionService);
            Assert.IsNotNull(requiredService.Value.CompositionService);

            var optionalService = fromRoot.Optional.CreateExport();
            Console.WriteLine("optionalService: {0}", optionalService.Value.CompositionService);
            Assert.IsNotNull(optionalService.Value.CompositionService);
        }

        [TestMethod]
        public void ScopingEndToEndWithCompositionService_MatchingCatalogsShouldSucceed()
        {
            var c = new TypeCatalog( typeof(ClassRoot), typeof(ClassA) );
            var sd = c.AsScope( c.AsScope() );

            var container = new CompositionContainer(sd, CompositionOptions.ExportCompositionService);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a1 = fromRoot.classA.CreateExport().Value;
            var a2 = fromRoot.classA.CreateExport().Value;
            fromRoot.localClassA.InstanceValue = 101;
            a1.InstanceValue = 202;
            a2.InstanceValue = 303;

            if (a1.InstanceValue ==  a2.InstanceValue) { throw new Exception("Incorrect sharing, a1 is shared with a2"); }

            var xroot = new ImportA();
            var x1 = new ImportA();
            var x2 = new ImportA();

            fromRoot.localClassA.CompositionService.SatisfyImportsOnce(xroot);
            a1.CompositionService.SatisfyImportsOnce(x1);
            a2.CompositionService.SatisfyImportsOnce(x2);
            Assert.AreEqual( xroot.classA.InstanceValue, fromRoot.localClassA.InstanceValue); 
            Assert.AreEqual( x1.classA.InstanceValue, a1.InstanceValue); 
            Assert.AreEqual( x2.classA.InstanceValue, a2.InstanceValue); 

        }

        [TestMethod]
        public void ScopingEndToEndWithCompositionService_PartitionedCatalogsShouldSucceed()
        {
            var c1 = new TypeCatalog( typeof(ClassRoot), typeof(ClassA) );
            var c2 = new TypeCatalog( typeof(ClassA) );
            var sd = c1.AsScope( c2.AsScope() );

            var container = new CompositionContainer(sd, CompositionOptions.ExportCompositionService);

            var fromRoot = container.GetExportedValue<ClassRoot>();
            var a1 = fromRoot.classA.CreateExport().Value;
            var a2 = fromRoot.classA.CreateExport().Value;
            fromRoot.localClassA.InstanceValue = 101;
            a1.InstanceValue = 202;
            a2.InstanceValue = 303;

            if (a1.InstanceValue ==  a2.InstanceValue) { throw new Exception("Incorrect sharing, a1 is shared with a2"); }

            var xroot = new ImportA();
            var x1 = new ImportA();
            var x2 = new ImportA();

            fromRoot.localClassA.CompositionService.SatisfyImportsOnce(xroot);
            a1.CompositionService.SatisfyImportsOnce(x1);
            a2.CompositionService.SatisfyImportsOnce(x2);
            Assert.AreEqual( xroot.classA.InstanceValue, fromRoot.localClassA.InstanceValue); 
            Assert.AreEqual( x1.classA.InstanceValue, a1.InstanceValue); 
            Assert.AreEqual( x2.classA.InstanceValue, a2.InstanceValue); 
        }

    }
}
