// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    public class TestAssemblyFour {}

    [Export]
    public class TestAssemblyOneExport {}

#if FEATURE_REFLECTIONCONTEXT
    // This is a glorious do nothing ReflectionContext
    class AssemblyCatalogTestsReflectionContext
    {
    }
#endif //FEATURE_REFLECTIONCONTEXT
}
