// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Virtual
{
    partial class VirtualPropertyBase
    {
        protected abstract class FuncPropertyAccessorBase : VirtualMethodBase
        {
            private readonly VirtualPropertyBase _declaringProperty;

            protected FuncPropertyAccessorBase(VirtualPropertyBase declaringProperty)
            {
                Contract.Requires(null != declaringProperty);

                _declaringProperty = declaringProperty;
            }

            public CustomReflectionContext ReflectionContext
            {
                get { return DeclaringProperty.ReflectionContext; }
            }

            public override sealed MethodAttributes Attributes
            {
                get { return base.Attributes | MethodAttributes.SpecialName; }
            }

            public override sealed Type DeclaringType
            {
                get { return _declaringProperty.DeclaringType; }
            }

            public VirtualPropertyBase DeclaringProperty
            {
                get { return _declaringProperty; }
            }
        }
    }
}
