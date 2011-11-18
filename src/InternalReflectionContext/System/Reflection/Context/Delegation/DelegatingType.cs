// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal abstract class DelegatingType : Type
	{
        private readonly Type _type;

        public DelegatingType(Type type)
        {
            Contract.Requires(null != type);

            _type = type;
        }

        public override Assembly Assembly
        {
            get { return _type.Assembly; }
        }

        public override string AssemblyQualifiedName
        {
            get { return _type.AssemblyQualifiedName; }
        }

        public override Type BaseType
        {
            get { return _type.BaseType; }
        }

        public override bool ContainsGenericParameters
        {
            get { return _type.ContainsGenericParameters; }
        }

        public override int GenericParameterPosition
        {
            get { return _type.GenericParameterPosition; }
        }

        public override MethodBase DeclaringMethod
        {
            get { return _type.DeclaringMethod; }
        }

        public override Type DeclaringType
        {
            get { return _type.DeclaringType; }
        }

        public override string FullName
        {
            get { return _type.FullName; }
        }

        public override GenericParameterAttributes GenericParameterAttributes
        {
            get { return _type.GenericParameterAttributes; }
        }

        public override Guid GUID
        {
            get { return _type.GUID; }
        }

        public override bool IsEnum
        {
            get { return _type.IsEnum; }
        }

        public override bool IsGenericParameter
        {
            get { return _type.IsGenericParameter; }
        }

        public override bool IsGenericType
        {
            get { return _type.IsGenericType; }
        }

        public override bool IsGenericTypeDefinition
        {
            get { return _type.IsGenericTypeDefinition; }
        }

        public override bool IsSecurityCritical
        {
            get { return _type.IsSecurityCritical; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return _type.IsSecuritySafeCritical; }
        }

        public override bool IsSecurityTransparent
        {
            get { return _type.IsSecurityTransparent; }
        }

        public override bool IsSerializable
        {
            get { return _type.IsSerializable; }
        }

        public override int MetadataToken
        {
            get { return _type.MetadataToken; }
        }

        public override Module Module
        {
            get { return _type.Module; }
        }

        public override string Name
        {
            get { return _type.Name; }
        }

        public override string Namespace
        {
            get { return _type.Namespace; }
        }

        public override Type ReflectedType
        {
            get { return _type.ReflectedType; }
        }

        public override StructLayoutAttribute StructLayoutAttribute
        {
            get { return _type.StructLayoutAttribute; }
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get { return _type.TypeHandle; }
        }

        public override Type UnderlyingSystemType
        {
            get { return _type.UnderlyingSystemType; }
        }

        public Type UnderlyingType
        {
            get { return _type; }
        }

        internal object Delegate
        {
            get { return UnderlyingType; }
        }

        public override int GetArrayRank()
        {
            return _type.GetArrayRank();
        }

        public override MemberInfo[] GetDefaultMembers()
        {
            return _type.GetDefaultMembers();
        }

        public override string GetEnumName(object value)
        {
            return _type.GetEnumName(value);
        }

        public override string[] GetEnumNames()
        {
            return _type.GetEnumNames();
        }

        public override Array GetEnumValues()
        {
            return _type.GetEnumValues();
        }

        public override Type GetEnumUnderlyingType()
        {
            return _type.GetEnumUnderlyingType();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _type.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _type.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _type.GetCustomAttributesData();
        }

        public override EventInfo[] GetEvents()
        {
            return _type.GetEvents();
        }

        public override Type[] GetGenericArguments()
        {
            return _type.GetGenericArguments();
        }

        public override Type[] GetGenericParameterConstraints()
        {
            return _type.GetGenericParameterConstraints();
        }

        public override Type GetGenericTypeDefinition()
        {
            return _type.GetGenericTypeDefinition();
        }

        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            return _type.GetInterfaceMap(interfaceType);
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            return _type.GetMember(name, type, bindingAttr);
        }

        protected override TypeCode GetTypeCodeImpl()
        {
            return Type.GetTypeCode(_type);
        }

        public override bool IsAssignableFrom(Type c)
        {
            return _type.IsAssignableFrom(c);
        }

        protected override bool IsContextfulImpl()
        {
            return _type.IsContextful;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _type.IsDefined(attributeType, inherit);
        }

        public override bool IsEnumDefined(object value)
        {
            return _type.IsEnumDefined(value);
        }

        public override bool IsEquivalentTo(Type other)
        {
            return _type.IsEquivalentTo(other);
        }

        public override bool IsInstanceOfType(object o)
        {
            return _type.IsInstanceOfType(o);
        }

        protected override bool IsMarshalByRefImpl()
        {
            return _type.IsMarshalByRef;
        }

        // We could have used the default implementation of this on Type
        // if it handled special cases like generic type constraints
        // and interfaces->objec.
        public override bool IsSubclassOf(Type c)
        {
            return _type.IsSubclassOf(c);
        }

        protected override bool IsValueTypeImpl()
        {
            return _type.IsValueType;
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return _type.Attributes;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return _type.GetConstructors(bindingAttr);
        }

        public override Type GetElementType()
        {
            return _type.GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _type.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _type.GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _type.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _type.GetFields(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _type.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return _type.GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return _type.GetMembers(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            // Unfortunately we cannot directly call the protected GetMethodImpl on _type.

            MethodInfo method;
            if (types == null)
            {
                method = _type.GetMethod(name, bindingAttr);
            }
            else
            {                
                method = _type.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
            }

            return method;
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return _type.GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return _type.GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return _type.GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return _type.GetProperties(bindingAttr);
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            // Unfortunately we cannot directly call the protected GetPropertyImpl on _type.
            PropertyInfo property;

            if (types == null)
            {
                // if types is null, we can ignore binder and modifiers

                if (returnType == null)
                    property = _type.GetProperty(name, bindingAttr);
                else
                {
                    // Ideally we should call a GetProperty overload that takes name, returnType, and bindingAttr, but not types.
                    // But such an overload doesn't exist. On the other hand, this also guarantees that bindingAttr will be
                    // the default lookup flags if types is null but returnType is not.
                    Contract.Assert(bindingAttr == (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));

                    property = _type.GetProperty(name, returnType);
                }
            }
            else
            {
                property = _type.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
            }
            
            return property;
        }

        protected override bool HasElementTypeImpl()
        {
            return _type.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return _type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        protected override bool IsArrayImpl()
        {
            return _type.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return _type.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return _type.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return _type.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return _type.IsPrimitive;
        }

        public override Type MakeArrayType()
        {
            return _type.MakeArrayType();
        }

        public override Type MakeArrayType(int rank)
        {
            return _type.MakeArrayType(rank);
        }

        public override Type MakePointerType()
        {
            return _type.MakePointerType();
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            return _type.MakeGenericType(typeArguments);
        }

        public override Type MakeByRefType()
        {
            return _type.MakeByRefType();
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}