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
    // Recursively 'projects' any assemblies, modules, types and members returned by a given property
    internal class ProjectingPropertyInfo : DelegatingPropertyInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingPropertyInfo(PropertyInfo property, Projector projector)
            : base(property)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region PropertyInfo overrides
        public override Type DeclaringType
        {
            get { return _projector.ProjectType(base.DeclaringType); }
        }

        public override Module Module
        {
            get { return _projector.ProjectModule(base.Module); }
        }

        public override Type PropertyType
        {
            get { return _projector.ProjectType(base.PropertyType); }
        }

        public override Type ReflectedType
        {
            get { return _projector.ProjectType(base.ReflectedType); }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return _projector.Project(base.GetAccessors(nonPublic), _projector.ProjectMethod);
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return _projector.ProjectMethod(base.GetGetMethod(nonPublic));
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return _projector.Project(base.GetIndexParameters(), _projector.ProjectParameter);
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _projector.ProjectMethod(base.GetSetMethod(nonPublic));
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

        public override Type[] GetOptionalCustomModifiers()
        {
            return _projector.Project(base.GetOptionalCustomModifiers(), _projector.ProjectType);
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return _projector.Project(base.GetRequiredCustomModifiers(), _projector.ProjectType);
        }
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingPropertyInfo other = o as ProjectingPropertyInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingProperty.Equals(other.UnderlyingProperty);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingProperty.GetHashCode();
        }
        #endregion
    }
}
