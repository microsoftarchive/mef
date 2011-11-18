// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;


namespace System.Reflection.Context.Virtual
{
    // Represents a inherited method which is identical to the base method except for its ReflectedType.
    internal partial class InheritedMethodInfo : DelegatingMethodInfo
    {
        private readonly Type _reflectedType;
        public InheritedMethodInfo(MethodInfo baseMethod, Type reflectedType)
            : base(baseMethod)
        {
            Contract.Requires(reflectedType != null);
            Contract.Requires(reflectedType.IsSubclassOf(baseMethod.DeclaringType));
            Contract.Requires(baseMethod is VirtualMethodBase);

            // Should we require that baseMethod is a declared method?
            Contract.Requires(baseMethod.ReflectedType.Equals(baseMethod.DeclaringType));

            _reflectedType = reflectedType;
        }

        public override Type ReflectedType
        {
            get
            {
                return _reflectedType;
            }
        }

        #region object overrides
        public override bool Equals(object o)
        {
            InheritedMethodInfo other = o as InheritedMethodInfo;
            return other != null &&
                   UnderlyingMethod.Equals(other.UnderlyingMethod) &&
                   ReflectedType.Equals(other.ReflectedType);
        }

        public override int GetHashCode()
        {
            return UnderlyingMethod.GetHashCode() ^ ReflectedType.GetHashCode();
        }
        #endregion
    }
}
