// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Internal
{
    internal static class LazyServices
    {
        public static Lazy<T> AsLazy<T>(this T t)
            where T : class
        {
            return new Lazy<T>(() => t, LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
