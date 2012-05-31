// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Composition
{
    /// <summary>
    ///     Specifies that a property, field, or parameter imports a particular export.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter,
                    AllowMultiple = false, Inherited = false)]
    public class ImportAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the 
        ///     export with the default contract name.
        /// </summary>
        public ImportAttribute()
            : this((string)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the
        ///     export with the contract name derived from the specified type.
        /// </summary>
        /// <param name="contractType">
        ///     A <see cref="Type"/> of which to derive the contract name of the export to import, or 
        ///     <see langword="null"/> to use the default contract name.
        /// </param>
        public ImportAttribute(Type contractType)
            : this((string)null, contractType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the
        ///     export with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the export to import, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        public ImportAttribute(string contractName)
            : this(contractName, (Type)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the
        ///     export with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the export to import, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        /// <param name="contractType">The contract type for the import.</param>
        public ImportAttribute(string contractName, Type contractType)
        {
            this.ContractName = contractName;
            this.ContractType = contractType;
        }

        /// <summary>
        ///     Gets the contract name of the export to import.
        /// </summary>
        /// <value>
        ///      A <see cref="String"/> containing the contract name of the export to import. The 
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

        /// <summary>
        ///     Gets or sets a value indicating whether the property, field or parameter will be set 
        ///     to its type's default value when an export with the contract name is not present in 
        ///     the container.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default value of a property's, field's or parameter's type is 
        ///         <see langword="null"/> for reference types and 0 for numeric value types. For 
        ///         other value types, the default value will be each field of the value type 
        ///         initialized to zero, if the field is a value type or <see langword="null"/> if 
        ///         the field is a reference type.
        ///     </para>
        /// </remarks>
        public bool AllowDefault { get; set; }
    }
}