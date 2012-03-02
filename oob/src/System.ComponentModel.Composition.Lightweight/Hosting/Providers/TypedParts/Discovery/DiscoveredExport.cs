// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    [DebuggerDisplay("{Contract}")]
    abstract class DiscoveredExport
    {
        readonly Contract _exportKey;
        readonly IDictionary<string, object> _metadata;
        DiscoveredPart _part;

        public DiscoveredExport(Contract exportKey, IDictionary<string, object> metadata)
        {
            _exportKey = exportKey;
            _metadata = metadata;
        }

        public Contract Contract { get { return _exportKey; } }

        public IDictionary<string, object> Metadata { get { return _metadata; } }

        public DiscoveredPart Part { get { return _part; } set { _part = value; } }

        public ExportDescriptorPromise GetExportDescriptorPromise(
            Contract contract,
            DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
               contract,
               Part.PartType.Name,
               Part.IsShared,
               () => Part.GetDependencies(definitionAccessor),
               deps =>
               {
                   var activator = Part.GetActivator(definitionAccessor, deps);
                   return GetExportDescriptor(activator);
               });
        }

        protected abstract ExportDescriptor GetExportDescriptor(CompositeActivator partActivator);

        public abstract DiscoveredExport CloseGenericExport(Type closedPartType, Type[] genericArguments);
    }
}
