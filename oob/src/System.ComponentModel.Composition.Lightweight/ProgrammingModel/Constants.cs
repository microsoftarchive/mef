// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.ProgrammingModel
{
    /// <summary>
    /// Metadata keys used to tie programming model entities into their back-end hosting implementations.
    /// </summary>
    static class Constants
    {
        public const string SharedPartMetadataName = "System.ComponentModel.Composition.Shared";
        public const string CreationPolicyPartMetadataName = "System.ComponentModel.Composition.CreationPolicy";

        public const string SharingBoundaryImportMetadataConstraintName = "SharingBoundaryNames";
        public const string ImportManyImportMetadataConstraintName = "IsImportMany";
        public const string KeyByMetadataImportMetadataConstraintName = "KeyMetadataName";
        public const string OrderByMetadataImportMetadataConstraintName = "OrderMetadataName";
    }
}
