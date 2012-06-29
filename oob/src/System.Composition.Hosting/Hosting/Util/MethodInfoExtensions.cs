// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Reflection;

namespace System.Composition.Hosting.Util
{
    static class MethodInfoExtensions
    {
        public static T CreateStaticDelegate<T>(this MethodInfo methodInfo)
        {
            return (T)(object)methodInfo.CreateDelegate(typeof(T));
        }
    }
}
