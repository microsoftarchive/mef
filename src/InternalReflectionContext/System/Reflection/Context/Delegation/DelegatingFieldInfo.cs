// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingFieldInfo : FieldInfo
	{
        private readonly FieldInfo _field;

        public DelegatingFieldInfo(FieldInfo field)
        {
            Contract.Requires(null != field);

            _field = field;
        }

        public override FieldAttributes Attributes
        {
            get { return _field.Attributes; }
        }

        public override Type DeclaringType
        {
            get { return _field.DeclaringType; }
        }

        public override RuntimeFieldHandle FieldHandle
        {
            get { return _field.FieldHandle; }
        }

        public override Type FieldType
        {
            get { return _field.FieldType; }
        }

        public override bool IsSecurityCritical
        {
            get { return _field.IsSecurityCritical; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return _field.IsSecuritySafeCritical; }
        }

        public override bool IsSecurityTransparent
        {
            get { return _field.IsSecurityTransparent; }
        }

        public override int MetadataToken
        {
            get { return _field.MetadataToken; }
        }

        public override Module Module
        {
            get { return _field.Module; }
        }

        public override string Name
        {
            get { return _field.Name; }
        }

        public override Type ReflectedType
        {
            get { return _field.ReflectedType; }
        }

        public FieldInfo UnderlyingField
        {
            get { return _field; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _field.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _field.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _field.GetCustomAttributesData();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return _field.GetOptionalCustomModifiers();
        }

        public override object GetRawConstantValue()
        {
            return _field.GetRawConstantValue();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return _field.GetRequiredCustomModifiers();
        }

        public override object GetValue(object obj)
        {
            return _field.GetValue(obj);
        }

        public override object GetValueDirect(TypedReference obj)
        {
            return _field.GetValueDirect(obj);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _field.IsDefined(attributeType, inherit);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            _field.SetValue(obj, value, invokeAttr, binder, culture);
        }

        public override void SetValueDirect(TypedReference obj, object value)
        {
            _field.SetValueDirect(obj, value);
        }

        public override string ToString()
        {
            return _field.ToString();
        }        
    }
}
