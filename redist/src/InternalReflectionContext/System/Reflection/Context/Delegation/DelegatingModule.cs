// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingModule : Module
    {
        private readonly Module _module;

        public DelegatingModule(Module module)
        {
            Contract.Requires(null != module);

            _module = module;
        }

        public Module UnderlyingModule
        {
            get { return _module; }
        }

        public override Assembly Assembly
        {
            get { return _module.Assembly; }
        }

        public override string FullyQualifiedName
        {
            get { return _module.FullyQualifiedName; }
        }

        public override int MDStreamVersion
        {
            get { return _module.MDStreamVersion; }
        }

        public override int MetadataToken
        {
            get { return _module.MetadataToken; }
        }

        public override Guid ModuleVersionId
        {
            get { return _module.ModuleVersionId; }
        }

        public override string Name
        {
            get { return _module.Name; }
        }

        public override string ScopeName
        {
            get { return _module.ScopeName; }
        }

        public override Type[] FindTypes(TypeFilter filter, object filterCriteria)
        {
            return _module.FindTypes(filter, filterCriteria);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _module.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _module.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _module.GetCustomAttributesData();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _module.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return _module.GetFields(bindingFlags);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            if (types == null)
            {
                return _module.GetMethod(name);
            }

            return _module.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);            
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
        {
            return _module.GetMethods(bindingFlags);
        }

        public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
        {
            _module.GetPEKind(out peKind, out machine);
        }

        public override X509Certificate GetSignerCertificate()
        {
            return _module.GetSignerCertificate();
        }

        public override Type GetType(string className, bool throwOnError, bool ignoreCase)
        {
            return _module.GetType(className, throwOnError, ignoreCase);
        }

        public override Type[] GetTypes()
        {
            return _module.GetTypes();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _module.IsDefined(attributeType, inherit);
        }

        public override bool IsResource()
        {
            return _module.IsResource();
        }

        public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return _module.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return _module.ResolveMember(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return _module.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override byte[] ResolveSignature(int metadataToken)
        {
            return _module.ResolveSignature(metadataToken);
        }

        public override string ResolveString(int metadataToken)
        {
            return _module.ResolveString(metadataToken);
        }

        public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return _module.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
        }

        public override string ToString()
        {
            return _module.ToString();
        }
    }
}