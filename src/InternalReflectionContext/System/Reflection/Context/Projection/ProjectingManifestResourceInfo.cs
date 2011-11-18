// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given resource
	internal class ProjectingManifestResourceInfo : DelegatingManifestResourceInfo
	{
        private readonly Projector _projector;

        public ProjectingManifestResourceInfo(ManifestResourceInfo resource, Projector projector)
            : base(resource)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public override Assembly ReferencedAssembly
        {
            get { return _projector.ProjectAssembly(base.ReferencedAssembly); }
        }
	}
}
