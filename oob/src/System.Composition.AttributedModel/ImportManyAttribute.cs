// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Composition
{
    /// <summary>
    ///     Specifies that a property, field, or parameter imports a particular set of exports.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter,
                    AllowMultiple = false, Inherited = false)]
    public class ImportManyAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the 
        ///     set of exports with the default contract name.
        /// </summary>
        public ImportManyAttribute()
            : this((string)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the
        ///     set of exports with the contract name derived from the specified type.
        /// </summary>
        /// <param name="contractType">
        ///     A <see cref="Type"/> of which to derive the contract name of the exports to import, or 
        ///     <see langword="null"/> to use the default contract name.
        /// </param>
        public ImportManyAttribute(Type contractType)
            : this((string)null, contractType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the
        ///     set of exports with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the exports to import, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        public ImportManyAttribute(string contractName)
            : this(contractName, (Type)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the
        ///     set of exports with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the exports to import, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        /// <param name="contractType">The contract type associated with the import.</param>
        public ImportManyAttribute(string contractName, Type contractType)
        {
            this.ContractName = contractName;
            this.ContractType = contractType;
        }

        /// <summary>
        ///     Gets the contract name of the exports to import.
        /// </summary>
        /// <value>
        ///      A <see cref="String"/> containing the contract name of the exports to import. The 
        ///      default value is an empty string ("").
        /// </value>
        public string ContractName { get; private set; }

        /// <summary>
        ///     Get the contract type of the export to import.
        /// </summary>
        /// <value>
        ///     A <see cref="Type"/> of the export that this import is expecting. The default value is
        ///     <see langword="null"/> which means that the type will be obtained by looking at the type on
        ///     the member that this import is attached to. If the type is <see cref="object"/> then the
        ///     importer is delaring they can accept any exported type.
        /// </value>
        public Type ContractType { get; private set; }
    }
}