// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Composition.Runtime.Util
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
            return string.Format("{0}<{1}>", name, string.Join(Properties.Resources.Formatter_ListSeparatorWithSpace, args));
        }
    }
}
