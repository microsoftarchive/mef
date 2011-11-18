// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingParameterInfo : ParameterInfo
	{
        private readonly ParameterInfo _parameter;

        public DelegatingParameterInfo(ParameterInfo parameter)
        {
            Contract.Requires(null != parameter);

            _parameter = parameter;
        }

        public override ParameterAttributes Attributes
        {
            get { return _parameter.Attributes; }
        }

        public override object DefaultValue
        {
            get { return _parameter.DefaultValue; }
        }

        public override MemberInfo Member
        {
            get { return _parameter.Member; }
        }

        public override int MetadataToken
        {
            get { return _parameter.MetadataToken; }
        }

        public override string Name
        {
            get { return _parameter.Name; }
        }

        public override Type ParameterType
        {
            get { return _parameter.ParameterType; }
        }

        public override int Position
        {
            get { return _parameter.Position; }
        }

        public override object RawDefaultValue
        {
            get { return _parameter.RawDefaultValue; }
        }

        public ParameterInfo UnderlyingParameter
        {
            get { return _parameter; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _parameter.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _parameter.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _parameter.GetCustomAttributesData();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return _parameter.GetOptionalCustomModifiers();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return _parameter.GetRequiredCustomModifiers();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _parameter.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _parameter.ToString();
        }
	}
}
