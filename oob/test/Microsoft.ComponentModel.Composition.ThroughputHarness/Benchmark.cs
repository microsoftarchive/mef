using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositionThroughput
{
    abstract class Benchmark
    {
        public abstract Action GetOperation();
        public abstract bool SelfTest();
        public virtual string Description { get { return GetType().Name.Replace("Benchmark", ""); } }
        public virtual Version Version { get { return new Version(); } }
    }
}
