// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Reflection.Context.Custom;

namespace System.Reflection.Context.Virtual
{
    partial class VirtualPropertyInfo
    {
        private class PropertyGetter : PropertyGetterBase
        {
            private readonly Func<object, object> _getter;
            private readonly IEnumerable<Attribute> _attributes;

            public PropertyGetter(VirtualPropertyBase property, Func<object, object> getter, IEnumerable<Attribute> getterAttributes)
                : base(property)
            {
                Contract.Requires(null != getter);

                _getter = getter;
                _attributes = getterAttributes ?? CollectionServices.Empty<Attribute>();
            }

            public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
            {
                // invokeAttr, binder, and culture are ignored, similar to what runtime reflection does with the default binder.

                if (parameters != null && parameters.Length > 0)
                    throw new TargetParameterCountException();

                if (!ReflectedType.IsInstanceOfType(obj))
                    throw new ArgumentException();

                return _getter(obj);
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
