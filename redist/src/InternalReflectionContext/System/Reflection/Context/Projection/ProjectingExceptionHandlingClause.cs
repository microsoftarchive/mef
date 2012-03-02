// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given exception handling clause
    internal class ProjectingExceptionHandlingClause : DelegatingExceptionHandlingClause
    {
        private readonly Projector _projector;

        public ProjectingExceptionHandlingClause(ExceptionHandlingClause clause, Projector projector)
            : base(clause)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public override Type CatchType
        {
            get { return _projector.ProjectType(base.CatchType); }
        }
    }
}
