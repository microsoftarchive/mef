// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;


namespace System.Reflection.Context.Virtual
{
    partial class VirtualPropertyBase
    {
        protected abstract class PropertyGetterBase : FuncPropertyAccessorBase
        {
            protected PropertyGetterBase(VirtualPropertyBase property)
                : base(property)
            {
            }

            public override sealed string Name
            {
                get { return "get_" + DeclaringProperty.Name; }
            }

            public override sealed Type ReturnType
            {
                get { return DeclaringProperty.PropertyType; }
            }

            protected override Type[] GetParameterTypes()
            {
                return CollectionServices.Empty<Type>();
            }
        }
    }
}
