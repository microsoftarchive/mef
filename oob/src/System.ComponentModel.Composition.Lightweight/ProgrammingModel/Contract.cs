// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.ProgrammingModel
{
    /// <summary>
    /// The link between exports and imports.
    /// </summary>
    /// <remarks>Extremely performance-sensitive.</remarks>
    public sealed class Contract
    {
        readonly Type _contractType;
        readonly object _discriminator;

        /// <summary>
        /// Construct a <see cref="Contract"/>.
        /// </summary>
        /// <param name="contractType">The type shared between the exporter and importer.</param>
        /// <param name="discriminator">Additional constraining information, or null.</param>
        public Contract(Type contractType, object discriminator = null)
        {
            _contractType = contractType;
            _discriminator = discriminator;
        }

        /// <summary>
        /// The type shared between the exporter and importer.
        /// </summary>
        public Type ContractType { get { return _contractType; } }

        /// <summary>
        /// Additional information that must be 100% agreed upon and commonly
        /// understood by the exporter and importer. The kinds of possible discriminator
        /// values (other than null) may be a string 'contract name', or more complex
        /// structures supporting Import Metadata. Extensions are free to create new
        /// kinds of discriminator without fear of clashes, however most kinds of
        /// discriminators should use the <see cref="MetadataConstrainedDiscriminator"/> protocol.
        /// </summary>
        public object Discriminator { get { return _discriminator; } }

        /// <summary>
        /// Determines equality between two contracts.
        /// </summary>
        /// <param name="obj">The contract to test.</param>
        /// <returns>True if the the contracts are equivalent; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var ek = obj as Contract;
            return ek != null && 
                ek._contractType.Equals(_contractType) &&
                (_discriminator == null ? ek._discriminator == null : _discriminator.Equals(ek._discriminator));
        }

        /// <summary>
        /// Gets a hash code for the contract.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _discriminator == null ?
                _contractType.GetHashCode() :
                _contractType.GetHashCode() ^ _discriminator.GetHashCode();
        }

        /// <summary>
        /// Creates a string representaiton of the contract.
        /// </summary>
        /// <returns>A string representaiton of the contract.</returns>
        public override string ToString()
        {
            var result = _contractType.Name.ToString();
            if (_discriminator != null)
                result += " " + Formatters.Format(_discriminator);

            return result;
        }
    }
}
