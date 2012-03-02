// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Used as a storage location for cached singleton
    /// values in <see cref="SharedInstanceOptimizationFeature"/>.
    /// </summary>
    sealed class GlobalInstanceHolder
    {
        #pragma warning disable 649 // unused
        public object Instance;
        #pragma warning restore 649
    }
}
