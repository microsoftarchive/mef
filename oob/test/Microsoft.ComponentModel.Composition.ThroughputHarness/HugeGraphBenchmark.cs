using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CompositionThroughput.HugeGraph;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Lightweight;

namespace CompositionThroughput
{
    abstract class HugeGraphBenchmark : Benchmark
    {
        public override bool SelfTest()
        {
            return true;
        }

        protected IEnumerable<Type> GetHugeGraphTypes(Type example)
        {
            return example.Assembly.GetTypes().Where(t => t.Namespace == example.Namespace);
        }
    }

    abstract class LightweightHugeGraphBenchmark : HugeGraphBenchmark
    {
        ExportFactory<T> ConfigureContainer<T>()
        {
            return new ContainerConfiguration()
                .WithParts(GetHugeGraphTypes(typeof(T)))
                .CreateContainer()
                .Value
                .GetExport<ExportFactory<T>>();
        }

        protected Action GetOperationFor<T>()
        {
            var c = ConfigureContainer<T>();
            return () =>
            {
                var scope = c.CreateExport();
                var x = scope.Value;
                scope.Dispose();
            };
        }

        public override Version Version
        {
            get
            {
                return typeof(IExportProvider).Assembly.GetName().Version;
            }
        }
    }

    class LightweightHugeGraphABenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassA1>();
        }
    }

    class LightweightLongGraphBBenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassB1>();
        }
    }

    class LightweightHugeGraphCBenchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassC1>();
        }
    }

    class LightweightHugeGraph4Benchmark : LightweightHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<HugeGraph4.TestClassA1>();
        }
    }
    
    abstract class MefHugeGraphBenchmark : HugeGraphBenchmark
    {
        CompositionContainer ConfigureContainer<T>()
        {
            var catalog = new TypeCatalog(GetHugeGraphTypes(typeof(T)));
            return new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
        }

        protected Action GetOperationFor<T>()
        {
            var c = ConfigureContainer<T>();
            return () =>
            {
                var x = c.GetExport<T>();
                var y = x.Value;
                c.ReleaseExport(x);
            };
        }

        public override Version Version
        {
            get
            {
                return typeof(CompositionContainer).Assembly.GetName().Version;
            }
        }
    }

    class MefHugeGraphABenchmark : MefHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassA1>();
        }
    }

    class MefHugeGraphBBenchmark : MefHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassB1>();
        }
    }

    class MefHugeGraphCBenchmark : MefHugeGraphBenchmark
    {
        public override Action GetOperation()
        {
            return GetOperationFor<TestClassC1>();
        }
    }
}
