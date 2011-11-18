// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Reflection.Context;

namespace System.Reflection.Context.Virtual
{
	internal abstract class VirtualMethodBase : MethodInfo
	{
        private ParameterInfo _returnParameter;

        protected VirtualMethodBase()
        {
        }

        #region defined methods
        protected abstract Type[] GetParameterTypes();
        #endregion

        #region overrides
        public override MethodAttributes Attributes
        {
            get { return MethodAttributes.Public | MethodAttributes.HideBySig; }
        }

        public override sealed CallingConventions CallingConvention
        {
            get { return CallingConventions.HasThis | CallingConventions.Standard; }
        }

        public override sealed bool ContainsGenericParameters
        {
            get { return false; }
        }

        public override sealed bool IsGenericMethod
        {
            get { return false; }
        }

        public override sealed bool IsGenericMethodDefinition
        {
            get { return false; }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotSupportedException(); }
        }

        public override sealed Module Module
        {
            get { return DeclaringType.Module; }
        }

        public override sealed Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override sealed ParameterInfo ReturnParameter
        {
            get { return _returnParameter ?? (_returnParameter = new VirtualReturnParameter(this)); }
        }

        public override sealed ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return ReturnParameter; }
        }

        public override sealed MethodInfo GetBaseDefinition()
        {
            return this;
        }

        public override sealed Type[] GetGenericArguments()
        {
            return CollectionServices.Empty<Type>();
        }

        public override sealed MethodInfo GetGenericMethodDefinition()
        {
            throw new InvalidOperationException();
        }

        public override sealed MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.IL;
        }

        public override ParameterInfo[] GetParameters()
        {
            return CollectionServices.Empty<ParameterInfo>();
        }

        public override sealed MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_NotGenericMethodDefinition, this));
        }
        #endregion

        #region ICustomAttributeProvider implementation
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return CollectionServices.Empty<object>();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return CollectionServices.Empty<object>();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return CollectionServices.Empty<CustomAttributeData>();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
        #endregion

        #region object override
        public override bool Equals(object obj)
        {
            VirtualMethodBase other = obj as VirtualMethodBase;
            if (other == null)
                return false;

            // We don't need to compare the invokees
            // But do we need to compare the contexts and return types?
            return
                Name == other.Name &&
                DeclaringType.Equals(other.DeclaringType) &&
                CollectionServices.CompareArrays(GetParameterTypes(), other.GetParameterTypes());
        }

        public override int GetHashCode()
        {
            return
                Name.GetHashCode() ^
                DeclaringType.GetHashCode() ^
                CollectionServices.GetArrayHashCode(GetParameterTypes());
        }

        public override string ToString()
        {
            StringBuilder toString = new StringBuilder();

            toString.Append(ReturnType.ToString());
            toString.Append(" ");
            toString.Append(Name);
            toString.Append("(");

            Type[] parameterTypes = GetParameterTypes();

            string comma = "";

            foreach (Type t in parameterTypes)
            {
                toString.Append(comma);
                toString.Append(t.ToString());

                comma = ", ";
            }

            if ((CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
            {
                toString.Append(comma);
                toString.Append("...");
            }

            return toString.ToString();
        }
        #endregion
    }
}
