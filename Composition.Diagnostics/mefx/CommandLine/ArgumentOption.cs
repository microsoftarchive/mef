//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace mefx.CommandLine
{
    class ArgumentOption<T> : Option
    {
        string _exampleArg;
        Action<T> _action;

        public ArgumentOption(string key, string exampleArg, string description, Action<T> action, IEnumerable<OptionGroup> groups)
            : base(key, description, groups)
        {
            _action = action;
            _exampleArg = exampleArg;
        }

        public override void AddValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            OptionPresent();

            _action((T)Convert.ChangeType(value, typeof(T)));
        }

        public override void PrintUsage(TextWriter writer)
        {
            var example = string.Format("/{0}:{1}", Key, _exampleArg);
            writer.WriteLine("  {0}", example);
            writer.WriteLine();
            writer.WriteLine("      {0}", Description);
            writer.WriteLine();
        }
    }
}
