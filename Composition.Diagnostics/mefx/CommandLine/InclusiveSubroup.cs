//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mefx.CommandLine
{
    class InclusiveSubroup : OptionGroup
    {
        OptionGroup[] _parentGroups;
        bool seen = false;

        public InclusiveSubroup(params OptionGroup[] parentGroups)
        {
            _parentGroups = parentGroups;
        }

        public override void OptionPresent(string key)
        {
            if (!seen)
            {
                seen = true;
                foreach (var p in _parentGroups)
                    p.OptionPresent(key);
            }
        }
    }
}
