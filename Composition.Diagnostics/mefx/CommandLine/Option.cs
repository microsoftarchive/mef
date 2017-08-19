//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mefx.CommandLine
{
    abstract class Option
    {
        string _key, _description;
        IEnumerable<OptionGroup> _groups;

        public Option(string key, string description, IEnumerable<OptionGroup> groups)
        {
            _key = key;
            _description = description;
            _groups = groups.ToArray();
        }

        public string Key { get { return _key; } }

        public string Description { get { return _description; } }

        public abstract void AddValue(string value);

        public abstract void PrintUsage(TextWriter writer);

        protected void OptionPresent()
        {
            foreach (var g in _groups)
                g.OptionPresent(Key);
        }
    }
}
