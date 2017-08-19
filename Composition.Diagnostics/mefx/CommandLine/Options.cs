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
    class Options
    {
        IDictionary<string, Option> _options = new Dictionary<string, Option>();

        public void Parse(string[] args)
        {
            foreach (var arg in args)
            {
                string argKey, argValue;
                ParseArg(arg, out argKey, out argValue);

                Option opt;
                if (!_options.TryGetValue(argKey, out opt))
                    throw new ArgumentException("Unrecognised option '" + argKey + "'");

                opt.AddValue(argValue);
            }
        }

        void ParseArg(string arg, out string key, out string value)
        {
            key = value = null;

            if (!arg.StartsWith("/"))
                throw new ArgumentException("Invalid command line input " + arg);

            var indexOfSemicolon = arg.IndexOf(':');
            if (indexOfSemicolon == -1)
            {
                key = arg.Substring(1);
            }
            else
            {
                key = arg.Substring(1, indexOfSemicolon - 1);
                if (indexOfSemicolon != arg.Length - 1)
                    value = arg.Substring(indexOfSemicolon + 1);
            }
        }

        public void Add<T>(string key, string description, string exampleArg, Action<T> valueProcessor, params OptionGroup[] groups)
        {
            _options.Add(key, new ArgumentOption<T>(key, description, exampleArg, valueProcessor, groups));
        }

        public void AddSwitch(string key, string description, Action switchOnAction, params OptionGroup[] groups)
        {
            _options.Add(key, new SwitchOption(key, description, switchOnAction, groups));
        }

        public void PrintUsage(TextWriter writer)
        {
            foreach (var opt in _options.Values.OrderBy(o => o.Key))
            {
                opt.PrintUsage(writer);
            }
        }
    }
}
