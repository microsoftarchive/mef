// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Used in conjunction with an ImportMany import,
    /// specifies the metadata item associated with each value that will be used to
    /// order the result.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    [MetadataAttribute]
    public class OrderByMetadataAttribute : Attribute
    {
        readonly string _metadataKey;

        /// <summary>
        /// Construct a <see cref="OrderByMetadataAttribute"/> for the specified metadata name.
        /// </summary>
        /// <param name="orderMetadataName">The name of the metadata item to use to order the collection.</param>
        public OrderByMetadataAttribute(string orderMetadataName)
        {
            _metadataKey = orderMetadataName;
        }

        /// <summary>
        /// The name of the metadata item to use as the key of the dictionary.
        /// </summary>
        public string OrderMetadataName { get { return _metadataKey; } }
    }
}
