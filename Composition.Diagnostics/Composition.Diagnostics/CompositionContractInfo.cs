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
    /// Describes use of a contract within a composition.
    /// </summary>
    public class CompositionContractInfo
    {
        public CompositionContractInfo(ContractInfo contract)
        {
            Contract = contract;
            Importers = new HashSet<PartDefinitionInfo>();
            Exporters = new HashSet<PartDefinitionInfo>();
        }

        /// <summary>
        /// The contract.
        /// </summary>
        public ContractInfo Contract { get; private set; }

        /// <summary>
        /// Importers of the contract.
        /// </summary>
        public ICollection<PartDefinitionInfo> Importers { get; private set; }

        /// <summary>
        /// Exporters of the contract.
        /// </summary>
        public ICollection<PartDefinitionInfo> Exporters { get; private set; }
    }
}
