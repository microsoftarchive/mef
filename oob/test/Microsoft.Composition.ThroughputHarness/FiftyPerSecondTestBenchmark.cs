using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompositionThroughput
{
    class FiftyPerSecondTestBenchmark : Benchmark
    {
        public override bool SelfTest()
        {
            return true;
        }

        public override string Description
        {
            get
            {
                return "Timing test 50/s";
            }
        }

        public override Action GetOperation()
        {
            return () => Thread.Sleep(1000 / 50);
        }
    }
}
