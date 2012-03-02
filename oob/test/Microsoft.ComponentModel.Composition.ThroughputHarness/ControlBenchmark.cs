using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositionThroughput
{
    class ControlBenchmark : Benchmark
    {
        public override Action GetOperation()
        {
            return () => { };
        }

        public override string Description
        {
            get { return "Control (Empty)"; }
        }

        public override bool SelfTest()
        {
            return true;
        }
    }
}
