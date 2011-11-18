// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Reflection.Context.Custom;
using System.Reflection.Context;

namespace System.Reflection.Context.Virtual
{
    partial class VirtualPropertyInfo
    {
        private class PropertySetter : PropertySetterBase
        {
            private readonly Action<object, object> _setter;
            private readonly ParameterInfo _valueParameter;
            private readonly IEnumerable<Attribute> _attributes;

            public PropertySetter(VirtualPropertyBase property, Action<object, object> setter, IEnumerable<Attribute> setterAttributes)
                : base(property)
            {
                Contract.Requires(null != setter);

                _setter = setter;
                _valueParameter = new VirtualParameter(this, property.PropertyType, "value", 0);
                _attributes = setterAttributes ?? CollectionServices.Empty<Attribute>();
            }

            public override ParameterInfo[] GetParameters()
            {
                return new ParameterInfo[] { _valueParameter };
            }

            public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
            {
                // invokeAttr, binder, and culture are ignored, similar to what runtime reflection does with the default binder.

                if (parameters == null || parameters.Length != 1)
                    throw new TargetParameterCountException();

                object value = parameters[0];

                if (obj == null)
                    throw new TargetException(SR.GetString(SR.Target_InstanceMethodRequiresTarget));

                if (!ReflectedType.IsInstanceOfType(obj))
                    throw new TargetException(SR.GetString(SR.Target_ObjectTargetMismatch));

                if (ReturnType.IsInstanceOfType(value))
                    throw new ArgumentException(SR.GetString(SR.Argument_ObjectArgumentMismatch, value.GetType(), ReturnType));

                _setter(obj, value);
                return null;
            }

            #region ICustomAttributeProvider implementation
            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                return CollectionServices.IEnumerableToArray(AttributeUtils.FilterCustomAttributes(_attributes, attributeType), attributeType);
            }

            public override object[] GetCustomAttributes(bool inherit)
            {
                return CollectionServices.IEnumerableToArray(_attributes, typeof(Attribute));
            }

            public override IList<CustomAttributeData> GetCustomAttributesData()
            {
                return CollectionServices.Empty<CustomAttributeData>();
            }

            public override bool IsDefined(Type attributeType, bool inherit)
            {
                return GetCustomAttributes(attributeType, inherit).Length > 0;
            }
            #endregion
        }
	}
}
