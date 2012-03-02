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
    class CycleBreakingExportDescriptor : ExportDescriptor
    {
        readonly Lazy<ExportDescriptor> _exportDescriptor;

        public CycleBreakingExportDescriptor(Lazy<ExportDescriptor> exportDescriptor)
        {
            _exportDescriptor = exportDescriptor;
        }

        public override CompositeActivator Activator
        {
            get
            {
                if (!_exportDescriptor.IsValueCreated)
                    return Activate;

                return _exportDescriptor.Value.Activator;
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                if (!_exportDescriptor.IsValueCreated)
                    return new CycleBreakingMetadataDictionary(_exportDescriptor);

                return _exportDescriptor.Value.Metadata;
            }
        }

        object Activate(LifetimeContext context, CompositionOperation operation)
        {
            if (!_exportDescriptor.IsValueCreated)
                throw new InvalidOperationException("Activation in progress before all descriptors fully initialized.");

            System.Diagnostics.Debug.WriteLine("[System.ComponentModel.Composition.Lightweight] Activating via cycle-breaking proxy.");
            return _exportDescriptor.Value.Activator(context, operation);
        }
    }
}
