//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T item)
        {
            return sequence.Concat(new[] { item });
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> sequence, T item, Func<bool> predicate)
        {
            if (predicate())
                return sequence.Append(item);
            else
                return sequence;
        }
    }
}
