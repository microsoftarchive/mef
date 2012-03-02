// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.ProgrammingModel
{
    /// <summary>
    /// When applied on an import, requires certain metadata values on the exporter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ImportMetadataConstraintAttribute : Attribute
    {
        readonly string _constraintName;
        readonly object _value;

        /// <summary>
        /// Require a specific metadata value on the exporter.
        /// </summary>
        /// <param name="constraintName">The metadata key to match.</param>
        /// <param name="value">The value to match.</param>
        public ImportMetadataConstraintAttribute(string constraintName, object value)
        {
            _constraintName = constraintName;
            _value = value;
        }

        /// <summary>
        /// The metadata key to match.
        /// </summary>
        public string ConstraintName { get { return _constraintName; } }

        /// <summary>
        /// The value to match.
        /// </summary>
        public object Value { get { return _value; } }
    }
}
