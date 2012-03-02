// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Represents a part property that is configured as an import.
    /// </summary>
    class PropertyImportSite
    {
        readonly PropertyInfo _pi;

        public PropertyImportSite(PropertyInfo pi)
        {
            _pi = pi;
        }

        public PropertyInfo Property { get { return _pi; } }

        public override string ToString()
        {
            return _pi.Name;
        }
    }
}
