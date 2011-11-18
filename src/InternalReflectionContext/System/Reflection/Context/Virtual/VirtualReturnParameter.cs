// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;


namespace System.Reflection.Context.Virtual
{
    // Represents the 'return' parameter for a method
    internal class VirtualReturnParameter : VirtualParameter
    {
        public VirtualReturnParameter(MethodInfo method)
            : base(method, method.ReturnType, name:null,  position: -1)
        {
        }
    }
}
