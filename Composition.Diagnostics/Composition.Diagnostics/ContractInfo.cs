//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Describes a typical attributed model contract.
    /// </summary>
    public class ContractInfo
    {
        /// <summary>
        /// Create a new ContractInfo.
        /// </summary>
        /// <param name="contractName">Contract name.</param>
        /// <param name="typeIdentity">Identity of the accepted export type.</param>
        /// <param name="metadata">Required metadata.</param>
        public ContractInfo(string contractName, string typeIdentity, IEnumerable<KeyValuePair<string, Type>> metadata)
        {
            ContractName = contractName;
            Metadata = metadata;
            TypeIdentity = typeIdentity;
        }

        /// <summary>
        /// The name associated with the contract.
        /// </summary>
        public string ContractName { get; private set; }

        /// <summary>
        /// The type identity exchanged between importers and exporters.
        /// </summary>
        public string TypeIdentity { get; private set; }

        /// <summary>
        /// Description of required metadata keys and value types.
        /// </summary>
        public IEnumerable<KeyValuePair<string, Type>> Metadata { get; private set; }

        /// <summary>
        /// Format the contract for display.
        /// </summary>
        /// <remarks>Excludes standard metadata items like PartCreationPolicy.</remarks>
        /// <example>
        ///   SomeContract : SomeType { Meta1 = foo, Meta2 = bar }
        /// </example>
        public string DisplayString
        {
            get
            {
                var result = new StringBuilder();
                result.Append(ContractName);
                if (TypeIdentity != null && TypeIdentity != ContractName)
                    result.Append(" : ").Append(TypeIdentity);

                var customMeta = Metadata.Where(m => !IsStandardMeta(m.Key));
                if (customMeta.Any())
                {
                    result.Append(" {");

                    var formatted = customMeta.Select(m =>
                        string.Format(" {0} : {1}", m.Key, m.Value));

                    result.Append(string.Join(",", formatted.ToArray()));

                    result.Append(" }");
                }

                return result.ToString();
            }
        }

        private bool IsStandardMeta(string key)
        {
            return key == CompositionConstants.ExportTypeIdentityMetadataName ||
                key == CompositionConstants.PartCreationPolicyMetadataName;
        }
    }
}
