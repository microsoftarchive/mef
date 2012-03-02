// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Util
{
    static class Formatters
    {
        public static string Format(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value is string)
                return "\"" + value + "\"";

            return value.ToString();
        }

        public static string ReadableQuotedList(IEnumerable<string> items)
        {
            return ReadableList(items.Select(i => "'" + i + "'"));
        }

        public static string ReadableList(IEnumerable<string> items)
        {
            var itemArray = items.ToArray();
            if (itemArray.Length == 0)
                return "<none>";

            if (itemArray.Length == 1)
                return itemArray[0];

            var ordered = itemArray.OrderByDescending(t => t).ToArray();
            var commaSeparated = ordered.Skip(1).Reverse();
            var last = ordered.First();
            return string.Join(", ", commaSeparated) + " and " + last;
        }
    }
}
