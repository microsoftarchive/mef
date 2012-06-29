// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Composition.Hosting.Core;

namespace System.Composition.TypedParts
{
    class ImportInfo
    {
        readonly CompositionContract _exportKey;
        readonly bool _allowDefault;

        public ImportInfo(CompositionContract exportKey, bool allowDefault)
        {
            _exportKey = exportKey;
            _allowDefault = allowDefault;
        }

        public bool AllowDefault { get { return _allowDefault; } }

        public CompositionContract Contract { get { return _exportKey; } }
    }
}
