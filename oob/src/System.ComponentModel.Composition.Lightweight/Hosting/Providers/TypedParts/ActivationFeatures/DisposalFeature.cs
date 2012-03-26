// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies the activator so that disposable instances are bound to the appropriate scope.
    /// </summary>
    class DisposalFeature : ActivationFeature
    {
        public override CompositeActivator RewriteActivator(
            Type partType,
            CompositeActivator activator, 
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            if (!typeof(IDisposable).IsAssignableFrom(partType))
                return activator;

            return (c, o) =>
            {
                var inst = (IDisposable)activator(c, o);
                c.AddBoundInstance(inst);
                return inst;
            };
        }
    }
}
