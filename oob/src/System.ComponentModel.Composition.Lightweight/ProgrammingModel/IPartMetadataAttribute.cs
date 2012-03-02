// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.ProgrammingModel
{
    /// <summary>
    /// Implemented on attributes that add metadata
    /// to parts.
    /// </summary>
    public interface IPartMetadataAttribute
    {
        /// <summary>
        /// The metadata applied to the part.
        /// </summary>
        IDictionary<string, object> Metadata { get; }
    }
}
