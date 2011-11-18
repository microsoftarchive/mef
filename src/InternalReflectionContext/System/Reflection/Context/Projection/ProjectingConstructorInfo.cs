// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given constructor
    internal class ProjectingConstructorInfo : DelegatingConstructorInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingConstructorInfo(ConstructorInfo constructor, Projector projector)
            : base(constructor)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region ConstructorInfo overrides
        public override Type DeclaringType
        {
            get { return _projector.ProjectType(base.DeclaringType); }
        }
    
        public override Module Module
        {
            get { return _projector.ProjectModule(base.Module); }
        }

        public override Type ReflectedType
        {
            get { return _projector.ProjectType(base.ReflectedType); }
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

        public override Type[] GetGenericArguments()
        {
            return _projector.Project(base.GetGenericArguments(), _projector.ProjectType);
        }

        public override MethodBody GetMethodBody()
        {
            return _projector.ProjectMethodBody(base.GetMethodBody());
        }

        public override ParameterInfo[] GetParameters()
        {
            return _projector.Project(base.GetParameters(), _projector.ProjectParameter);
        }
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingConstructorInfo other = o as ProjectingConstructorInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingConstructor.Equals(other.UnderlyingConstructor);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingConstructor.GetHashCode();
        }
        #endregion
    }
}
