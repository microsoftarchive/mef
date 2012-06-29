// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Internal;

namespace System.Composition.Hosting.Core
{
    class DirectExportDescriptor : ExportDescriptor
    {
        readonly CompositeActivator _activator;
        readonly IDictionary<string, object> _metadata;

        public DirectExportDescriptor(CompositeActivator activator, IDictionary<string, object> metadata)
        {
            Requires.ArgumentNotNull(activator, "activator");
            Requires.ArgumentNotNull(metadata, "metadata");

            _activator = activator;
            _metadata = metadata;
        }

        public override CompositeActivator Activator { get { return _activator; } }

        public override IDictionary<string, object> Metadata { get { return _metadata; } }
    }
}
