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
    // Recursively 'projects' any assemblies, modules, types and members returned by a given method
    internal class ProjectingMethodInfo : DelegatingMethodInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingMethodInfo(MethodInfo method, Projector projector)
            : base(method)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region MethodInfo overrides
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

        public override ParameterInfo ReturnParameter
        {
            get { return _projector.ProjectParameter(base.ReturnParameter); }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get
            {
                // We should just return MethodInfo.ReturnParameter here
                // but DynamicMethod returns a fake ICustomAttributeProvider.
                ICustomAttributeProvider provider = base.ReturnTypeCustomAttributes;
                ParameterInfo parameter = provider as ParameterInfo;
                if (parameter != null)
                    return _projector.ProjectParameter(ReturnParameter);
                else
                    return provider;
            }
        }

        public override Type ReturnType
        {
            get { return _projector.ProjectType(base.ReturnType); }
        }
      
        public override MethodInfo GetBaseDefinition()
        {
            return _projector.ProjectMethod(base.GetBaseDefinition());
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

        public override MethodInfo GetGenericMethodDefinition()
        {
            return _projector.ProjectMethod(base.GetGenericMethodDefinition());
        }

        public override MethodBody GetMethodBody()
        {
            return _projector.ProjectMethodBody(base.GetMethodBody());
        }

        public override ParameterInfo[] GetParameters()
        {
            return _projector.Project(base.GetParameters(), _projector.ProjectParameter);
        }
   
        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            return _projector.ProjectMethod(base.MakeGenericMethod(_projector.Unproject(typeArguments)));            
        }
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingMethodInfo other = o as ProjectingMethodInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingMethod.Equals(other.UnderlyingMethod);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingMethod.GetHashCode();
        }
        #endregion
    }
}
