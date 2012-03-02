// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;
using System.Reflection.Context;

namespace System.Reflection.Context.Virtual
{
	internal class VirtualParameter : ParameterInfo
	{
        public VirtualParameter(MemberInfo member, Type parameterType, string name, int position)
        {
            if (parameterType == null)
                throw new ArgumentNullException("parameterType");
            if (member == null)
                throw new ArgumentNullException("member");

            Contract.Assert(position >= -1);

            ClassImpl = parameterType;
            MemberImpl = member;
            NameImpl = name;
            PositionImpl = position;
        }

        internal static ParameterInfo[] CloneParameters(MemberInfo member, ParameterInfo[] parameters, bool skipLastParameter)
        {
            int length = parameters.Length;
            if (skipLastParameter)
            {
                length--;
            }

            ParameterInfo[] clonedParameters = new ParameterInfo[length];

            for (int i = 0; i < length; i++)
            {
                ParameterInfo parameter = parameters[i];
                clonedParameters[i] = new VirtualParameter(member, parameter.ParameterType, parameter.Name, parameter.Position);
            }

            return clonedParameters;
        }

        #region object overrides
        public override bool Equals(object obj)
        {
            VirtualParameter other = obj as VirtualParameter;

            if (other == null)
                return false;

            // Do we need to compare Name and ParameterType?
            return
                Member == other.Member &&
                Position == other.Position &&
                ParameterType == other.ParameterType;
        }

        public override int GetHashCode()
        {
            return
                Member.GetHashCode() ^
                Position.GetHashCode() ^
                ParameterType.GetHashCode();
        }
        #endregion
	}
}
