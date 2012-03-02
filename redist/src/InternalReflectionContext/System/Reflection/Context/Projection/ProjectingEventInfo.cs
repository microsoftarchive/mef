// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given event
    internal class ProjectingEventInfo : DelegatingEventInfo, IProjectable
	{
        private readonly Projector _projector;

        public ProjectingEventInfo(EventInfo @event, Projector projector)
            : base(@event)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region EventInfo overrides
        public override Type DeclaringType
        {
            get { return _projector.ProjectType(base.DeclaringType); }
        }

        public override Type EventHandlerType
        {
            get { return _projector.ProjectType(base.EventHandlerType); }
        }

        public override Module Module
        {
            get { return _projector.ProjectModule(base.Module); }
        }

        public override Type ReflectedType
        {
            get { return _projector.ProjectType(base.ReflectedType); }
        }

        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            return _projector.ProjectMethod(base.GetAddMethod(nonPublic));
        }

        public override MethodInfo[] GetOtherMethods(bool nonPublic)
        {
            return _projector.Project(base.GetOtherMethods(nonPublic), _projector.ProjectMethod);
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            return _projector.ProjectMethod(base.GetRaiseMethod(nonPublic));
        }

        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            return _projector.ProjectMethod(base.GetRemoveMethod(nonPublic));
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
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingEventInfo other = o as ProjectingEventInfo;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingEvent.Equals(other.UnderlyingEvent);
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingEvent.GetHashCode();
        }
        #endregion
    }
}
