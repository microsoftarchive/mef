using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.UnitTests.Util;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class CircularityTests : ContainerTests
    {
        public interface IA { }

        [Export, PartCreationPolicy(CreationPolicy.Shared)]
        public class BLazy
        {
            public Lazy<IA> A;

            [ImportingConstructor]
            public BLazy(Lazy<IA> ia)
            {
                A = ia;
            }
        }

        [Export(typeof(IA))]
        public class ACircular : IA
        {
            public BLazy B;

            [ImportingConstructor]
            public ACircular(BLazy b)
            {
                B = b;
            }
        }

        [Export, PartCreationPolicy(CreationPolicy.Shared)]
        public class PropertyPropertyA
        {
            [Import]
            public PropertyPropertyB B { get; set; }
        }

        [Export]
        public class PropertyPropertyB
        {
            [Import]
            public PropertyPropertyA A { get; set; }
        }

        [Export]
        public class ConstructorPropertyA
        {
            [Import]
            public ConstructorPropertyB B { get; set; }
        }

        [Export, PartCreationPolicy(CreationPolicy.Shared)]
        public class ConstructorPropertyB
        {
            [ImportingConstructor]
            public ConstructorPropertyB(ConstructorPropertyA a)
            {
                A = a;
            }

            public ConstructorPropertyA A { get; private set; }
        }

        public interface ICircularM { string Name { get; } }

        [Export, ExportMetadata("Name", "A")]
        public class MetadataCircularityA
        {
            [Import]
            public Lazy<MetadataCircularityB, ICircularM> B { get; set; }
        }

        [Export, ExportMetadata("Name", "B"), PartCreationPolicy(CreationPolicy.Shared)]
        public class MetadataCircularityB
        {
            [Import]
            public Lazy<MetadataCircularityA, ICircularM> A { get; set; }
        }

        [Export, PartCreationPolicy(CreationPolicy.Shared)]
        public class NonPrereqSelfDependency
        {
            [Import]
            public NonPrereqSelfDependency Self { get; set; }
        }

        [Export]
        public class PrDepA
        {
            [ImportingConstructor]
            public PrDepA(PrDepB b) { }
        }

        [Export]
        public class PrDepB
        {
            [ImportingConstructor]
            public PrDepB(PrDepA a) { }
        }

        [TestMethod]
        public void CanHandleDefinitionCircularity()
        {
            var cc = CreateContainer(typeof(ACircular), typeof(BLazy));
            var x = cc.GetExport<BLazy>();
            Assert.IsInstanceOfType(x.A.Value, typeof(ACircular));
            Assert.IsInstanceOfType(((ACircular)x.A.Value).B, typeof(BLazy));
        }

        [TestMethod]
        public void CanHandleDefinitionCircularity2()
        {
            var cc = CreateContainer(typeof(ACircular), typeof(BLazy));
            var x = cc.GetExport<IA>();
            Assert.IsInstanceOfType(((ACircular)((ACircular)x).B.A.Value).B, typeof(BLazy));
        }

        [TestMethod]
        public void HandlesPropertyPropertyCircularity()
        {
            var cc = CreateContainer(typeof(PropertyPropertyA), typeof(PropertyPropertyB));
            var a = cc.GetExport<PropertyPropertyA>();
            Assert.AreSame(a.B.A, a);
        }

        [TestMethod]
        public void HandlesPropertyPropertyCircularityReversed()
        {
            var cc = CreateContainer(typeof(PropertyPropertyA), typeof(PropertyPropertyB));
            var b = cc.GetExport<PropertyPropertyB>();
            Assert.AreSame(b.A.B, b.A.B.A.B);
        }

        [TestMethod]
        public void HandlesConstructorPropertyCircularity()
        {
            var cc = CreateContainer(typeof(ConstructorPropertyA), typeof(ConstructorPropertyB));
            var a = cc.GetExport<ConstructorPropertyA>();
            Assert.AreSame(a.B.A.B.A, a.B.A);
        }

        [TestMethod]
        public void HandlesConstructorPropertyCircularityReversed()
        {
            var cc = CreateContainer(typeof(ConstructorPropertyA), typeof(ConstructorPropertyB));
            var b = cc.GetExport<ConstructorPropertyB>();
            Assert.AreSame(b, b.A.B);
        }

        [TestMethod]
        public void HandlesMetadataCircularity()
        {
            var cc = CreateContainer(typeof(MetadataCircularityA), typeof(MetadataCircularityB));
            var a = cc.GetExport<MetadataCircularityA>();

            Assert.AreEqual(a.B.Metadata.Name, "B");
            Assert.AreEqual(a.B.Value.A.Metadata.Name, "A");
        }

        [TestMethod]
        public void SharedPartCanHaveNonPrereqDependencyOnSelf()
        {
            var cc = CreateContainer(typeof(NonPrereqSelfDependency));
            var npsd = cc.GetExport<NonPrereqSelfDependency>();
            Assert.AreSame(npsd, npsd.Self);
        }

        [TestMethod]
        public void PrerequisiteCircularitiesAreDetected()
        {
            var cc = CreateContainer(typeof(PrDepA), typeof(PrDepB));

            var x = AssertX.Throws<LightweightCompositionException>(() =>
            {
                cc.GetExport<PrDepA>();
            });
        }
    }
}
