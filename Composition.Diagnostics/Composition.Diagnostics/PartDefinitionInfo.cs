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
    /// Analysis of a part definition within the composition.
    /// </summary>
    public class PartDefinitionInfo
    {
        Lazy<bool> _isRejected;

        /// <summary>
        ///  Create a PartDefinitionInfo representing partDefinition. 
        /// </summary>
        /// <param name="partDefinition">The part to be analyzed.</param>
        public PartDefinitionInfo(ComposablePartDefinition partDefinition)
        {
            PartDefinition = partDefinition;
            ImportDefinitions = Enumerable.Empty<ImportDefinitionInfo>();
            _isRejected = new Lazy<bool>(() => ImportDefinitions
                    .Any(idi => idi.Exception is ImportCardinalityMismatchException));
        }

        /// <summary>
        /// The part definition represented by this instance.
        /// </summary>
        public ComposablePartDefinition PartDefinition { get; private set; }

        /// <summary>
        /// True if the part is rejected.
        /// </summary>
        public bool IsRejected
        {
            get
            {
                return _isRejected.Value;
            }
        }

        /// <summary>
        /// Information on each of the part definition's imports.
        /// </summary>
        public IEnumerable<ImportDefinitionInfo> ImportDefinitions { get; set; }

        /// <summary>
        /// True if this part is rejected and the reasons for rejection cannot be
        /// attributed to the rejection of other parts.
        /// </summary>
        public bool IsPrimaryRejection
        {
            get
            {
                return ImportDefinitions.Where(id => id.IsBrokenAndNotRejected).Any();
            }
        }

        /// <summary>
        /// Because we traverse a potentially cyclic graph
        /// use this to keep things simple.
        /// </summary>
        const int DefaultMaximumAnalysisDepth = 20;

        /// <summary>
        /// If the part is rejected, find the most likely parts in the composition
        /// that caused this rejection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PartDefinitionInfo> FindPossibleRootCauses()
        {
            return FindPossibleRootCauses(DefaultMaximumAnalysisDepth);
        }

        /// <summary>
        /// If the part is rejected, find the most likely parts in the composition
        /// that caused this rejection.
        /// </summary>
        /// <param name="maximumAnalysisDepth">The maximum depth of the composition
        /// graph to be inspected.</param>
        /// <returns></returns>
        public IEnumerable<PartDefinitionInfo> FindPossibleRootCauses(int maximumAnalysisDepth)
        {
            return ImportDefinitions
                .Where(id => id.IsBroken)
                .SelectMany(id => id
                    .UnsuitableExportDefinitions
                    .Where(ed => ed.PartDefinition.IsRejected && ed.PartDefinition != this)
                    .SelectMany(ed => ed.PartDefinition.FindPossibleRootCauses(--maximumAnalysisDepth))
                    .Take(maximumAnalysisDepth))
                .Append(this)
                .Where(pd => pd.IsPrimaryRejection)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Determine whether the part imports a contract.
        /// </summary>
        /// <param name="contract">Name of the contract to check for.</param>
        /// <returns>True if the part imports the contract.</returns>
        public bool ImportsContract(string contract)
        {
            return ImportDefinitions.Any(id => id.ImportDefinition.ContractName == contract);
        }

        /// <summary>
        /// Determine whether the part exports a contract.
        /// </summary>
        /// <param name="contract">Name of the contract to check for.</param>
        /// <returns>True if the part exports the contract.</returns>
        public bool ExportsContract(string contract)
        {
            return PartDefinition.ExportDefinitions.Any(ed => ed.ContractName == contract);
        }
    }
}
