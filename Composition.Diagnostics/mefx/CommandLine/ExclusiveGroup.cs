//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mefx.CommandLine
{
    class ExclusiveGroup : OptionGroup
    {
        string keyUsed;

        public override void OptionPresent(string key)
        {
            if (keyUsed == null)
                keyUsed = key;
            else if (keyUsed != key)
                throw new ArgumentException(string.Format(
                    "The options '{0}' and '{1}' cannot be specified together.",
                    keyUsed,
                    key));
        }
    }
}
