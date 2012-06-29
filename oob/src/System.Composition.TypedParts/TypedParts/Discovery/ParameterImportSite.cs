// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Reflection;

namespace System.Composition.TypedParts.Discovery
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
