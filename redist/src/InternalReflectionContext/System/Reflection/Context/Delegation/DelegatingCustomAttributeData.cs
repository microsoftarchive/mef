// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingCustomAttributeData : CustomAttributeData
	{
        private readonly CustomAttributeData _attribute;

        public DelegatingCustomAttributeData(CustomAttributeData attribute)
        {
            Contract.Requires(null != attribute);

            _attribute = attribute;
        }

        public CustomAttributeData UnderlyingAttribute
        {
            get { return _attribute; }
        }

        public override ConstructorInfo Constructor
        {
            get { return _attribute.Constructor; }
        }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments
        {
            get { return _attribute.ConstructorArguments; }
        }

        public override IList<CustomAttributeNamedArgument> NamedArguments
        {
            get { return _attribute.NamedArguments; }
        }

        public override string ToString()
        {
            return _attribute.ToString();
        }
    }
}
