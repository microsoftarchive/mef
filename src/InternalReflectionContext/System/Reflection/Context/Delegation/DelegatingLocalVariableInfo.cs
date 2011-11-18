// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingLocalVariableInfo : LocalVariableInfo
    {
        private readonly LocalVariableInfo _variable;

        public DelegatingLocalVariableInfo(LocalVariableInfo variable)
        {
            Contract.Requires(variable != null);

            _variable = variable;
        }

        public override bool IsPinned
        {
            get { return _variable.IsPinned; }
        }

        public override int LocalIndex
        {
            get { return _variable.LocalIndex; }
        }

        public override Type LocalType
        {
            get { return _variable.LocalType; }
        }

        public LocalVariableInfo UnderlyingVariable
        {
            get { return _variable; }
        }

        public override string ToString()
        {
            return _variable.ToString();
        }
    }
}
