// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingEventInfo : EventInfo
    {
        private readonly EventInfo _event;

        public DelegatingEventInfo(EventInfo @event)
        {
            Contract.Requires(null != @event);

            _event = @event;
        }

        public override EventAttributes Attributes
        {
            get { return _event.Attributes; }
        }

        public override Type DeclaringType
        {
            get { return _event.DeclaringType; }
        }

        public override Type EventHandlerType
        {
            get { return _event.EventHandlerType; }
        }

        public override bool IsMulticast
        {
            get { return _event.IsMulticast; }
        }

        public override int MetadataToken
        {
            get { return _event.MetadataToken; }
        }

        public override Module Module
        {
            get { return _event.Module; }
        }

        public override string Name
        {
            get { return _event.Name; }
        }

        public override Type ReflectedType
        {
            get { return _event.ReflectedType; }
        }

        public EventInfo UnderlyingEvent
        {
            get { return _event; }
        }

        public override void AddEventHandler(object target, Delegate handler)
        {
            _event.AddEventHandler(target, handler);
        }

        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            return _event.GetAddMethod(nonPublic);
        }

        public override MethodInfo[] GetOtherMethods(bool nonPublic)
        {
            return _event.GetOtherMethods(nonPublic);
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            return _event.GetRaiseMethod(nonPublic);
        }

        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            return _event.GetRemoveMethod(nonPublic);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _event.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _event.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _event.GetCustomAttributesData();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _event.IsDefined(attributeType, inherit);
        }

        public override void RemoveEventHandler(object target, Delegate handler)
        {
            _event.RemoveEventHandler(target, handler);
        }

        public override string ToString()
        {
            return _event.ToString();
        }
    }
}
