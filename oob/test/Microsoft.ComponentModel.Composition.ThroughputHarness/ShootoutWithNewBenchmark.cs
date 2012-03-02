using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Lightweight;
using System.ComponentModel.Composition.Hosting;

namespace CompositionThroughput
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class X { }

    [Export]
    public class XFactory
    {
        readonly ExportFactory<X> _xfactory;

        [ImportingConstructor]
        public XFactory(ExportFactory<X> xfactory)
        {
            _xfactory = xfactory;
        }

        public ExportLifetimeContext<X> CreateX()
        {
            return _xfactory.CreateExport();
        }
    }

    abstract class ShootoutWithNewBenchmark : Benchmark
    {
        public override bool SelfTest()
        {
            return true;
        }
    }

    class OperatorNewBenchmark : ShootoutWithNewBenchmark
    {
        public override Action GetOperation()
        {
            return () => new X();
        }
    }

    class LightweightNewBenchmark : ShootoutWithNewBenchmark
    {
        public override Action GetOperation()
        {
            var c = new ContainerConfiguration()
                .WithPart(typeof(X))
                .WithPart(typeof(XFactory))
                .CreateContainer();

            var xf = c.Value.GetExport<XFactory>();
            return () =>
            {
                var x = xf.CreateX();
                var unused = x.Value;
                x.Dispose();
            };
        }
    }

    class LightweightNLNewBenchmark : ShootoutWithNewBenchmark
    {
        public override Action GetOperation()
        {
            var c = new ContainerConfiguration()
                .WithPart(typeof(X))
                .CreateContainer();

            return () =>
            {
                c.Value.GetExport<X>();
            };
        }
    }

    class MefNewBenchmark : ShootoutWithNewBenchmark
    {
        public override Action GetOperation()
        {
            var c = new TypeCatalog(typeof(X), typeof(XFactory));
            var container = new CompositionContainer(c, true);
            var xf = container.GetExportedValue<XFactory>();
            return () =>
            {
                var x = xf.CreateX();
                var unused = x.Value;
                x.Dispose();
            };
        }
    }

    class MefNLNewBenchmark : ShootoutWithNewBenchmark
    {
        public override Action GetOperation()
        {
            var c = new TypeCatalog(typeof(X), typeof(XFactory));
            var container = new CompositionContainer(c, true);
            return () =>
            {
                container.GetExportedValue<X>();
            };
        }
    }
}
