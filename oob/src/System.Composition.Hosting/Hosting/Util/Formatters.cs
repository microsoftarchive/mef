// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace System.Composition.Hosting.Util
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

        public static string Format(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsConstructedGenericType)
                return FormatClosedGeneric(type);

            return type.Name;
        }

        static string FormatClosedGeneric(Type closedGenericType)
        {
            if (closedGenericType == null) throw new ArgumentNullException("closedGenericType");
            if (!closedGenericType.IsConstructedGenericType) throw new ArgumentException();
            var name = closedGenericType.Name.Substring(0, closedGenericType.Name.IndexOf("`"));
            var args = closedGenericType.GenericTypeArguments.Select(t => Format(t));
            return string.Format("{0}<{1}>", name, string.Join(", ", args));
        }
    }
}
