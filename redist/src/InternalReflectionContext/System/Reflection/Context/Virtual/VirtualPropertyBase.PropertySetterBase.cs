// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;


namespace System.Reflection.Context.Virtual
{
    partial class VirtualPropertyBase
    {
        protected abstract class PropertySetterBase : FuncPropertyAccessorBase
        {
            private Type[] _parameterTypes;

            protected PropertySetterBase(VirtualPropertyBase property)
                : base(property)
            {
            }

            public override sealed string Name
            {
                get { return "set_" + DeclaringProperty.Name; }
            }

            public override sealed Type ReturnType
            {
                get { return DeclaringProperty.ReflectionContext.MapType(typeof(void)); }
            }

            protected override Type[] GetParameterTypes()
            {
                return _parameterTypes != null ?
                       _parameterTypes :
                       _parameterTypes = new Type[1] { DeclaringProperty.PropertyType };
            }
        }
    }
}
