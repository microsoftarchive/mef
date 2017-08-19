//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mefx.CommandLine
{
    abstract class OptionGroup
    {
        public abstract void OptionPresent(string key);
    }
}
