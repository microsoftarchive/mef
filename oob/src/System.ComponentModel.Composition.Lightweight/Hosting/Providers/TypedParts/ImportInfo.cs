// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts
{
    class ImportInfo
    {
        readonly Contract _exportKey;
        readonly bool _allowDefault;

        public ImportInfo(Contract exportKey, bool allowDefault)
        {
            _exportKey = exportKey;
            _allowDefault = allowDefault;
        }

        public bool AllowDefault { get { return _allowDefault; } }

        public Contract Contract { get { return _exportKey; } }
    }
}
