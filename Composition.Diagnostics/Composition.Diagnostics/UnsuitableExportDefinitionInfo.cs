//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Describes an export that has the same contract name as
    /// required by the target import but which cannot be used to
    /// satisfy it.
    /// </summary>
    public class UnsuitableExportDefinitionInfo
    {
        IEnumerable<UnsuitableExportDefinitionIssue> _issues;

        /// <summary>
        /// Create an UnsuitableExportDefinitionInfo.
        /// </summary>
        /// <param name="target">Import for which the export is unsuitable.</param>
        /// <param name="exportDefinition">Unsuitable export.</param>
        /// <param name="partDefinition">Part definition that provided the export.</param>
        public UnsuitableExportDefinitionInfo(
            ContractBasedImportDefinition target,
            ExportDefinition exportDefinition,
            PartDefinitionInfo partDefinition)
        {
            ExportDefinition = exportDefinition;
            PartDefinition = partDefinition;
            _issues = CheckForRequiredMetadataIssues(target, exportDefinition)
                            .Concat(CheckForTypeIdentityIssues(target, exportDefinition))
                            .Concat(CheckForCreationPolicyIssues(target, partDefinition.PartDefinition))
                            .ToArray();
        }

        /// <summary>
        /// Export definition found to be unsuitable.
        /// </summary>
        public ExportDefinition ExportDefinition { get; private set; }

        /// <summary>
        /// Part definition providing the unsuitable export.
        /// </summary>
        public PartDefinitionInfo PartDefinition { get; private set; }

        /// <summary>
        /// Reasons for the export being unusable.
        /// </summary>
        public IEnumerable<UnsuitableExportDefinitionIssue> Issues
        {
            get
            {
                var added = new UnsuitableExportDefinitionIssue[0];

                if (PartDefinition.IsRejected)
                    added = new[] {
                        new UnsuitableExportDefinitionIssue(
                            UnsuitableExportDefinitionReason.PartDefinitionIsRejected,
                            "The part providing the export is rejected because of other issues.")
                    };

                return _issues.Concat(added);
            }
        }

        static IEnumerable<UnsuitableExportDefinitionIssue> CheckForCreationPolicyIssues(ContractBasedImportDefinition cbid, ComposablePartDefinition pd)
        {
            if (cbid.RequiredCreationPolicy != CreationPolicy.Any)
            {
                object actual;
                if (pd.Metadata.TryGetValue(CompositionConstants.PartCreationPolicyMetadataName, out actual) &&
                    actual != null)
                {
                    if (actual is CreationPolicy && !actual.Equals(cbid.RequiredCreationPolicy))
                    {
                        yield return new UnsuitableExportDefinitionIssue(
                            UnsuitableExportDefinitionReason.CreationPolicy,
                            string.Format("The import requires creation policy '{0}', but the exporting part only supports '{1}'.",
                                cbid.RequiredCreationPolicy, actual));
                    }
                    else
                    {
                        yield return new UnsuitableExportDefinitionIssue(
                            UnsuitableExportDefinitionReason.CreationPolicy,
                            string.Format("The metadata stored for creation policy should be of type CreationPolicy, but is '{0}'.", actual.GetType()));
                    }
                }
            }
        }

        private static IEnumerable<UnsuitableExportDefinitionIssue> CheckForRequiredMetadataIssues(
            ContractBasedImportDefinition cbid, ExportDefinition ped)
        {
            var missing = cbid
                .RequiredMetadata
                .Where(rm => !ped.Metadata.ContainsKey(rm.Key))
                .Select(rm => new UnsuitableExportDefinitionIssue(
                                    UnsuitableExportDefinitionReason.RequiredMetadata,
                                    string.Format("The import requires metadata '{0}' but this is not provided by the export.", rm)));

             var typeMismatch = cbid
                    .RequiredMetadata
                    .Join(
                        ped.Metadata,
                        rm => rm.Key,
                        m => m.Key,
                        (rm, m) => new { Key = rm.Key, RequiredType = rm.Value, Actual = m.Value })
                    .Where(info => info.Actual != null && !info.RequiredType.IsAssignableFrom(info.Actual.GetType()))
                    .Select(info => new UnsuitableExportDefinitionIssue(
                                    UnsuitableExportDefinitionReason.RequiredMetadata,
                                    string.Format("The import requires metadata '{0}' to be of type '{1}' but the provided value '{2}' is of type '{3}'",
                                        info.Key, info.RequiredType, info.Actual, info.Actual.GetType())));

             return missing.Concat(typeMismatch);
        }

        private static IEnumerable<UnsuitableExportDefinitionIssue> CheckForTypeIdentityIssues(
            ContractBasedImportDefinition cbid, ExportDefinition ped)
        {
            if (cbid.RequiredTypeIdentity != null)
            {
                object actual;
                if (ped.Metadata.TryGetValue(
                    CompositionConstants.ExportTypeIdentityMetadataName, out actual) &&
                    actual != null)
                {
                    var actualString = actual as string;
                    if (actualString == null)
                    {
                        yield return new UnsuitableExportDefinitionIssue(
                            UnsuitableExportDefinitionReason.TypeIdentity,
                            string.Format("ExportTypeIdentity is provided, but the metadata value is a '{0}'. The value should be a string.",
                                actual.GetType()));
                    }
                    else if (cbid.RequiredTypeIdentity != actualString)
                    {
                        yield return new UnsuitableExportDefinitionIssue(
                            UnsuitableExportDefinitionReason.TypeIdentity,
                            string.Format("The export is a '{0}', but the import requires '{1}'. These types must match exactly (conversions are not supported.)",
                                actualString, cbid.RequiredTypeIdentity));
                    }
                }
                else
                {
                    yield return new UnsuitableExportDefinitionIssue(
                        UnsuitableExportDefinitionReason.TypeIdentity,
                        string.Format("Import requires '{0}', but export does not provide type information. This should be stored in ExportDefinition.Metadata[{1}].",
                            cbid.RequiredTypeIdentity, CompositionConstants.ExportTypeIdentityMetadataName));
                }
            }
        }
    }

}
