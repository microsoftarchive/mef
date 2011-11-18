// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given method body
    internal class ProjectingMethodBody : DelegatingMethodBody
    {
        private readonly Projector _projector;

        public ProjectingMethodBody(MethodBody body, Projector projector)
            : base(body)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public override IList<ExceptionHandlingClause> ExceptionHandlingClauses
        {
            get { return _projector.Project(base.ExceptionHandlingClauses, _projector.ProjectExceptionHandlingClause); }
        }

        public override IList<LocalVariableInfo> LocalVariables
        {
            get { return _projector.Project(base.LocalVariables, _projector.ProjectLocalVariable); }
        }
    }
}
