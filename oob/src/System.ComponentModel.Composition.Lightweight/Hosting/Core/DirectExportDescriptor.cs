// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    class DirectExportDescriptor : ExportDescriptor
    {
        readonly CompositeActivator _activator;
        readonly IDictionary<string, object> _metadata;

        public DirectExportDescriptor(CompositeActivator activator, IDictionary<string, object> metadata)
        {
            if (activator == null) throw new ArgumentNullException("activator");
            if (metadata == null) throw new ArgumentNullException("metadata");

            _activator = activator;
            _metadata = metadata;
        }

        public override CompositeActivator Activator { get { return _activator; } }

        public override IDictionary<string, object> Metadata { get { return _metadata; } }
    }
}
