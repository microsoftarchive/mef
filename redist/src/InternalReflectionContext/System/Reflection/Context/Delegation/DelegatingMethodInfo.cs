// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given method
    internal class DelegatingMethodInfo : MethodInfo
	{
        private readonly MethodInfo _method;

        public DelegatingMethodInfo(MethodInfo method)
        {
            Contract.Requires(null != method);

            _method = method;
        }

        public override MethodAttributes Attributes
        {
            get { return _method.Attributes; }
        }

        public override CallingConventions CallingConvention
        {
            get { return _method.CallingConvention; }
        }

        public override bool ContainsGenericParameters
        {
            get { return _method.ContainsGenericParameters; }
        }

        public override Type DeclaringType
        {
            get { return _method.DeclaringType; }
        }

        public override bool IsGenericMethod
        {
            get { return _method.IsGenericMethod; }
        }

        public override bool IsGenericMethodDefinition
        {
            get { return _method.IsGenericMethodDefinition; }
        }

        public override bool IsSecurityCritical
        {
            get { return _method.IsSecurityCritical; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return _method.IsSecuritySafeCritical; }
        }

        public override bool IsSecurityTransparent
        {
            get { return _method.IsSecurityTransparent; }
        }

        public override int MetadataToken
        {
            get { return _method.MetadataToken; }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return _method.MethodHandle; }
        }

        public override Module Module
        {
            get { return _method.Module; }
        }

        public override string Name
        {
            get { return _method.Name; }
        }

        public override Type ReflectedType
        {
            get { return _method.ReflectedType; }
        }

        public override ParameterInfo ReturnParameter
        {
            get { return _method.ReturnParameter; }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return _method.ReturnTypeCustomAttributes; }
        }

        public override Type ReturnType
        {
            get { return _method.ReturnType; }
        }

        public MethodInfo UnderlyingMethod
        {
            get { return _method; }
        }

        public override MethodInfo GetBaseDefinition()
        {
            return _method.GetBaseDefinition();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _method.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _method.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _method.GetCustomAttributesData();
        }

        public override Type[] GetGenericArguments()
        {
            return _method.GetGenericArguments();
        }

        public override MethodInfo GetGenericMethodDefinition()
        {
            return _method.GetGenericMethodDefinition();
        }

        public override MethodBody GetMethodBody()
        {
            return _method.GetMethodBody();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _method.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            return _method.Invoke(obj, invokeAttr, binder, parameters, culture);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _method.IsDefined(attributeType, inherit);
        }

        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            return _method.MakeGenericMethod(typeArguments);            
        }

        public override string ToString()
        {
            return _method.ToString();
        }        
    }
}
