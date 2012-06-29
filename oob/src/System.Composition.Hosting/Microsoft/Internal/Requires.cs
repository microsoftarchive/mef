// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

namespace Microsoft.Internal
{
    using System.Diagnostics;
    using System.Composition.Hosting.Properties;

    static class Requires
    {
        [DebuggerStepThrough]
        static public void ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw ThrowHelper.ArgumentNullException(argumentName);
            }
        }
    }
}