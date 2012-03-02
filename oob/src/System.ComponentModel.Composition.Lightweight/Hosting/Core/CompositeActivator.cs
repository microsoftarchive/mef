// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// The delegate signature that allows instances of parts and exports to be accessed during
    /// a composition operation.
    /// </summary>
    /// <param name="context">The context in which the part or export is being accessed.</param>
    /// <param name="operation">The operation within which the activation is occuring.</param>
    /// <returns>The activated part or export.</returns>
    public delegate object CompositeActivator(LifetimeContext context, CompositionOperation operation);
}
