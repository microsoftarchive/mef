//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.ComponentModel.Composition.Diagnostics;

namespace mefx
{
    /// <summary>
    /// Keep track of permissible rejections. Used by the /whitelist switch.
    /// </summary>
    class RejectionWhitelist
    {
        ICollection<string> _items = new HashSet<string>();
        bool _allowAll = false;

        /// <summary>
        /// Creates a whitelist that permits all parts to be rejected.
        /// </summary>
        public RejectionWhitelist()
        {
            _allowAll = true;
        }

        /// <summary>
        /// Read lines from a text file describing the allowable rejections.
        /// </summary>
        /// <param name="sourceFile">A file of the format produced by
        /// the output of the /rejected command.</param>
        public RejectionWhitelist(string sourceFile)
        {
            foreach (var item in File.ReadAllLines(sourceFile)
                .Select(i => i.Trim()))
                if (!_items.Contains(item))
                    _items.Add(item);
        }

        /// <summary>
        /// True if the part can be rejected.
        /// </summary>
        /// <param name="pd">Part to test.</param>
        /// <returns>True if rejection is acceptable.</returns>
        public bool IsRejectionAllowed(PartDefinitionInfo pd)
        {
            return _allowAll || 
                _items.Contains(CompositionElementTextFormatter.DisplayCompositionElement(pd.PartDefinition));
        }
    }
}
