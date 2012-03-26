// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Util;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies the activators of parts so that they a) get associated with the correct
    /// scope, and b) obtain their dependencies from the correct scope.
    /// </summary>
    class LifetimeFeature  : ActivationFeature
    {
        public override CompositeActivator RewriteActivator(
            Type partType,
            CompositeActivator activatorBody,
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            if (!ContractHelpers.IsShared(partMetadata))
                return activatorBody;

            object sharingBoundaryMetadata;
            if (!partMetadata.TryGetValue(Constants.SharedPartMetadataName, out sharingBoundaryMetadata))
                sharingBoundaryMetadata = null;

            var sharingBoundary = (string)sharingBoundaryMetadata;
            var sharingKey = LifetimeContext.AllocateSharingId();

            return (c, o) =>
            {
                var scope = c.FindContextWithin(sharingBoundary);
                if (object.ReferenceEquals(scope, c))
                    return scope.GetOrCreate(sharingKey, o, activatorBody);
                else
                    return CompositionOperation.Run(scope, (c1, o1) => c1.GetOrCreate(sharingKey, o1, activatorBody));
            };
        }
    }
}
 