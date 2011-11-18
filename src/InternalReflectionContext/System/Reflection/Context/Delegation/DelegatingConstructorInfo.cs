// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingConstructorInfo : ConstructorInfo
    {
        private readonly ConstructorInfo _constructor;

        public DelegatingConstructorInfo(ConstructorInfo constructor)
        {
            Contract.Requires(null != constructor);

            _constructor = constructor;
        }

        public override MethodAttributes Attributes
        {
            get { return _constructor.Attributes; }
        }

        public override CallingConventions CallingConvention
        {
            get { return _constructor.CallingConvention; }
        }

        public override bool ContainsGenericParameters
        {
            get { return _constructor.ContainsGenericParameters; }
        }

        public override Type DeclaringType
        {
            get { return _constructor.DeclaringType; }
        }

        public override bool IsGenericMethod
        {
            get { return _constructor.IsGenericMethod; }
        }

        public override bool IsGenericMethodDefinition
        {
            get { return _constructor.IsGenericMethodDefinition; }
        }

        public override bool IsSecurityCritical
        {
            get { return _constructor.IsSecurityCritical; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return _constructor.IsSecuritySafeCritical; }
        }

        public override bool IsSecurityTransparent
        {
            get { return _constructor.IsSecurityTransparent; }
        }

        public override int MetadataToken
        {
            get { return _constructor.MetadataToken; }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return _constructor.MethodHandle; }
        }

        public override Module Module
        {
            get { return _constructor.Module; }
        }

        public override string Name
        {
            get { return _constructor.Name; }
        }

        public override Type ReflectedType
        {
            get { return _constructor.ReflectedType; }
        }

        public ConstructorInfo UnderlyingConstructor
        {
            get { return _constructor; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _constructor.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _constructor.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _constructor.GetCustomAttributesData();
        }

        public override Type[] GetGenericArguments()
        {
            return _constructor.GetGenericArguments();
        }

        public override MethodBody GetMethodBody()
        {
            return _constructor.GetMethodBody();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _constructor.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _constructor.GetParameters();
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            return _constructor.Invoke(invokeAttr, binder, parameters, culture);
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            return _constructor.Invoke(obj, invokeAttr, binder, parameters, culture);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _constructor.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _constructor.ToString();
        }
    }
}
