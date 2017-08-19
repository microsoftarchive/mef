//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Categorization of reasons for exports not being usable.
    /// </summary>
    public enum UnsuitableExportDefinitionReason
    {
        None,

        /// <summary>
        /// Problems with requried metadata.
        /// </summary>
        RequiredMetadata,

        /// <summary>
        /// The part supplying the export is rejected.
        /// </summary>
        PartDefinitionIsRejected,

        /// <summary>
        /// Problems with the creation policy.
        /// </summary>
        CreationPolicy,

        /// <summary>
        /// Problems with the type identity.
        /// </summary>
        TypeIdentity
    }
}
