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
    // Recursively 'projects' any assemblies, modules, types and members for a given field
    internal class ProjectingFieldInfo : DelegatingFieldInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingFieldInfo(FieldInfo field, Projector projector)
            : base(field)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region FieldInfo overrides
        public override Type DeclaringType
        {
            get { return _projector.ProjectType(base.DeclaringType); }
        }

        public override Type FieldType
        {
            get { return _projector.ProjectType(base.FieldType); }
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
            ProjectingFieldInfo other = o as ProjectingFieldInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingField.Equals(other.UnderlyingField);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingField.GetHashCode();
        }
        #endregion
    }
}
