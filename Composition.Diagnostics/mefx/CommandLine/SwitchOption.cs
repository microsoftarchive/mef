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
    class SwitchOption : Option
    {
        Action _action;

        public SwitchOption(string key, string description, Action action, IEnumerable<OptionGroup> groups)
            : base(key, description, groups)
        {
            _action = action;
        }

        public override void AddValue(string value)
        {
            if (value != null && value.Trim() != "")
                throw new ArgumentException("An argument cannot be supplied to option '" + Key + "'");

            OptionPresent();

            _action();
        }

        public override void PrintUsage(TextWriter writer)
        {
            writer.WriteLine("  /{0} ", Key);
            writer.WriteLine();
            writer.WriteLine("      {0}", Description);
            writer.WriteLine();
        }
    }
}
