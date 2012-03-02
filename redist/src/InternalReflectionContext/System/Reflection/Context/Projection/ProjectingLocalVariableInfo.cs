// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given variable
    internal class ProjectingLocalVariableInfo : DelegatingLocalVariableInfo
    {
        private readonly Projector _projector;

        public ProjectingLocalVariableInfo(LocalVariableInfo variable, Projector projector)
            : base(variable)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }
    
        public override Type LocalType
        {
            get { return _projector.ProjectType(base.LocalType); }
        }
    }
}
