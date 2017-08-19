//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Analysis of an import attached to a part definition.
    /// </summary>
    public class ImportDefinitionInfo
    {
        public ImportDefinitionInfo(
            ImportDefinition importDefinition,
            Exception exception,
            IEnumerable<ExportDefinition> actuals,
            IEnumerable<UnsuitableExportDefinitionInfo> unsuitableExportDefinitions)
        {
            ImportDefinition = importDefinition;
            Exception = exception;
            Actuals = actuals.ToArray();
            UnsuitableExportDefinitions = unsuitableExportDefinitions.ToArray();
        }

        /// <summary>
        /// The import definition.
        /// </summary>
        public ImportDefinition ImportDefinition { get; private set; }

        /// <summary>
        /// True if the import fails within the composition being analyzed.
        /// </summary>
        public bool IsBroken { get { return Exception != null; } }

        /// <summary>
        /// If IsBroken, contains the exception thrown when trying to satisfy the
        /// import. Otherwise null.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// If !IsBroken, the export definitions that would satisfy the import
        /// in the composition being analyzed. Otherwise empty.
        /// </summary>
        public IEnumerable<ExportDefinition> Actuals { get; private set; }

        /// <summary>
        /// Exports that match the contract name of the import, but cannot be used.
        /// For example, exports with incorrect type identities, incorrect creation policies,
        /// invalid metadata or come from rejected parts.  
        /// </summary>
        public IEnumerable<UnsuitableExportDefinitionInfo> UnsuitableExportDefinitions { get; private set; }
 
        /// <summary>
        /// True if the import cannot be satisfied for reasons other than the rejection
        /// of exporting parts (i.e. it is potentially the 'root cause' of a composition
        /// failure.)
        /// </summary>
        public bool IsBrokenAndNotRejected
        {
            get
            {
                return Exception is ImportCardinalityMismatchException &&
                    !(UnsuitableExportDefinitions.Any(ued =>
                        ued.PartDefinition.IsRejected));
            }
        }

        /// <summary>
        /// Describes the contract being imported.
        /// </summary>
        public ContractInfo ContractInfo
        {
            get
            {
                var rm = Enumerable.Empty<KeyValuePair<string, Type>>();

                var cbid = ImportDefinition as ContractBasedImportDefinition;
                if (cbid != null)
                    rm = cbid.RequiredMetadata;

                return new ContractInfo(
                    ImportDefinition.ContractName,
                    RequiredTypeIdentity(ImportDefinition),
                    rm);
            }
        }

        static string RequiredTypeIdentity(ImportDefinition importDefinition)
        {
            var cbid = importDefinition as ContractBasedImportDefinition;
            if (cbid != null)
                return cbid.RequiredTypeIdentity;
            else
                return null;
        }
    }
}
