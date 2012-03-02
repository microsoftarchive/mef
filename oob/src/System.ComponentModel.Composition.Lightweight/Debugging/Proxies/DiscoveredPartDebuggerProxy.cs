// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Debugging.Proxies
{
    class DiscoveredPartDebuggerProxy
    {
        DiscoveredPart _discoveredPart;

        public DiscoveredPartDebuggerProxy(DiscoveredPart discoveredPart)
        {
            _discoveredPart = discoveredPart;
        }

        public Type PartType
        {
            get { return _discoveredPart.PartType; }
        }

        public DiscoveredExport[] Exports
        {
            get { return _discoveredPart.DiscoveredExports.ToArray(); }
        }
    }
}
