// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingManifestResourceInfo : ManifestResourceInfo
	{
        private readonly ManifestResourceInfo _resource;

        public DelegatingManifestResourceInfo(ManifestResourceInfo resource)
            : base(resource.ReferencedAssembly, resource.FileName, resource.ResourceLocation)
        {
            Contract.Requires(null != resource);

            _resource = resource;
        }

        public ManifestResourceInfo UnderlyingResource
        {
            get { return _resource; }
        }
	}
}
