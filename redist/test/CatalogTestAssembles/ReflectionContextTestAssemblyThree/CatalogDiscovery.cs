// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    public class TestAssemblyThree {}

    [Export]
    public class TestAssemblyOneExport {}

#if FEATURE_REFLECTIONCONTEXT
    // This is a glorious do nothing ReflectionContext
    public class ReflectionContextTestAssemblyThreeReflectionContext : ReflectionContext
    {
        private ReflectionContextTestAssemblyThreeReflectionContext() {}
        public override Assembly MapAssembly(Assembly assembly)
        {
            return assembly;
        }

#if FEATURE_INTERNAL_REFLECTIONCONTEXT
        public override Type MapType(Type type)
#else
        public override TypeInfo MapType(TypeInfo type)
#endif
        {
            return type;
        }
   }
#endif
}
