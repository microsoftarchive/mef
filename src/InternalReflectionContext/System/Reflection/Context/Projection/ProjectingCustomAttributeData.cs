// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given custom attribute data
	internal class ProjectingCustomAttributeData : DelegatingCustomAttributeData
	{
        private readonly Projector _projector;

        public ProjectingCustomAttributeData(CustomAttributeData attribute, Projector projector)
            : base(attribute)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public override ConstructorInfo Constructor
        {
            get { return _projector.ProjectConstructor(base.Constructor); }
        }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments
        {
            get { return _projector.Project(base.ConstructorArguments, _projector.ProjectTypedArgument); }
        }

        public override IList<CustomAttributeNamedArgument> NamedArguments
        {
            get { return _projector.Project(base.NamedArguments, _projector.ProjectNamedArgument); }
        }
    }
}
