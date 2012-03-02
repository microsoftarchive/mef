// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    class ParameterImportSite
    {
        readonly ParameterInfo _pi;

        public ParameterImportSite(ParameterInfo pi)
        {
            _pi = pi;
        }

        public ParameterInfo Parameter { get { return _pi; } }

        public override string ToString()
        {
            return _pi.Name;
        }
    }
}
