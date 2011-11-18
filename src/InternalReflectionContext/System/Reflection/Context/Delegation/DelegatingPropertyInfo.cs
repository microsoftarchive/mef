// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingPropertyInfo : PropertyInfo
    {
        private readonly PropertyInfo _property;

        public DelegatingPropertyInfo(PropertyInfo property)
        {
            Contract.Requires(null != property);

            _property = property;
        }

        public override PropertyAttributes Attributes
        {
            get { return _property.Attributes; }
        }

        public override bool CanRead
        {
            get { return _property.CanRead; }
        }

        public override bool CanWrite
        {
            get { return _property.CanWrite; }
        }

        public override Type DeclaringType
        {
            get { return _property.DeclaringType; }
        }

        public override int MetadataToken
        {
            get { return _property.MetadataToken; }
        }

        public override Module Module
        {
            get { return _property.Module; }
        }

        public override string Name
        {
            get { return _property.Name; }
        }

        public override Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public override Type ReflectedType
        {
            get { return _property.ReflectedType; }
        }

        public PropertyInfo UnderlyingProperty
        {
            get { return _property; }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return _property.GetAccessors(nonPublic);
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return _property.GetGetMethod(nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return _property.GetIndexParameters();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _property.GetSetMethod(nonPublic);
        }

        public override object GetValue(object obj, object[] index)
        {
            return _property.GetValue(obj, index);
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            return _property.GetValue(obj, invokeAttr, binder, index, culture);
        }

        public override void SetValue(object obj, object value, object[] index)
        {
            _property.SetValue(obj, value, index);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            _property.SetValue(obj, value, invokeAttr, binder, index, culture);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _property.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _property.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _property.GetCustomAttributesData();
        }

        public override object GetConstantValue()
        {
            return _property.GetConstantValue();
        }

        public override object GetRawConstantValue()
        {
            return _property.GetRawConstantValue();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return _property.GetOptionalCustomModifiers();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return _property.GetRequiredCustomModifiers();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _property.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _property.ToString();
        }
    }
}
