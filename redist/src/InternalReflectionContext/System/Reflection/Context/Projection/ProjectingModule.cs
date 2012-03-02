// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given module
    internal class ProjectingModule : DelegatingModule, IProjectable
    {
        private readonly Projector _projector;

        public ProjectingModule(Module module, Projector projector)
            : base(module)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region Module overrides
        public override Assembly Assembly
        {
            get { return _projector.ProjectAssembly(base.Assembly); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            attributeType = _projector.Unproject(attributeType);

            return base.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _projector.Project(base.GetCustomAttributesData(), _projector.ProjectCustomAttributeData);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            attributeType = _projector.Unproject(attributeType);

            return base.IsDefined(attributeType, inherit);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _projector.ProjectField(base.GetField(name, bindingAttr));
        }

        public override FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return _projector.Project(base.GetFields(bindingFlags), _projector.ProjectField);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            types = _projector.Unproject(types);
            return _projector.ProjectMethod(base.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers));
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
        {
            return _projector.Project(base.GetMethods(bindingFlags), _projector.ProjectMethod);
        }

        public override Type GetType(string className, bool throwOnError, bool ignoreCase)
        {
            return _projector.ProjectType(base.GetType(className, throwOnError, ignoreCase));
        }

        public override Type[] GetTypes()
        {
            return _projector.Project(base.GetTypes(), _projector.ProjectType);
        }

        public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            genericTypeArguments = _projector.Unproject(genericTypeArguments);
            genericMethodArguments = _projector.Unproject(genericMethodArguments);

            return _projector.ProjectField(base.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments));
        }

        public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            genericTypeArguments = _projector.Unproject(genericTypeArguments);
            genericMethodArguments = _projector.Unproject(genericMethodArguments);

            return _projector.ProjectMember(base.ResolveMember(metadataToken, genericTypeArguments, genericMethodArguments));
        }

        public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            genericTypeArguments = _projector.Unproject(genericTypeArguments);
            genericMethodArguments = _projector.Unproject(genericMethodArguments);

            return _projector.ProjectMethodBase(base.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments));
        }

        public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            genericTypeArguments = _projector.Unproject(genericTypeArguments);
            genericMethodArguments = _projector.Unproject(genericMethodArguments);

            return _projector.ProjectType(base.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments));
        }
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingModule other = o as ProjectingModule;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingModule.Equals(other.UnderlyingModule);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingModule.GetHashCode();
        }
        #endregion
    }
}
