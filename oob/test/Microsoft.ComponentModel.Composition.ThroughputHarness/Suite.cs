using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositionThroughput
{
    class Suite
    {
        readonly string _name;
        readonly int _standardRunOperations;
        readonly Benchmark[] _includedBenchmarks;

        public Suite(string name, int standardRunOperations, Benchmark[] includedBenchmarks)
        {
            _name = name; _standardRunOperations = standardRunOperations; _includedBenchmarks = includedBenchmarks;
        }

        public string Name { get { return _name; } }

        public int StandardRunOperations { get { return _standardRunOperations; } }

        public Benchmark[] IncludedBenchmarks { get { return _includedBenchmarks; } }
    }
}
