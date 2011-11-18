// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given parameter
    internal class ProjectingParameterInfo : DelegatingParameterInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingParameterInfo(ParameterInfo parameter, Projector projector)
            : base(parameter)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region ParameterInfo overrides
        public override MemberInfo Member
        {
            get { return _projector.ProjectMember(base.Member); }
        }

        public override Type ParameterType
        {
            get { return _projector.ProjectType(base.ParameterType); }
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
            ProjectingParameterInfo other = o as ProjectingParameterInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingParameter.Equals(other.UnderlyingParameter);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingParameter.GetHashCode();
        }
        #endregion
    }
}
