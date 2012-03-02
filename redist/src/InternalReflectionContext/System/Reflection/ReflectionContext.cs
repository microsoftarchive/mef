// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

namespace System.Reflection
{
    using System;

    public abstract class ReflectionContext
    {
        protected ReflectionContext() { }

        public abstract Assembly MapAssembly(Assembly assembly);

        public abstract Type MapType(Type type);

        public virtual Type GetTypeForObject(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return MapType(value.GetType());
        }
    }
}