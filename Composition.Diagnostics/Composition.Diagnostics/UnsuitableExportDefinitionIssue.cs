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
    /// A reason for an export definition not satisfying a particular import.
    /// </summary>
    public class UnsuitableExportDefinitionIssue
    {
        /// <summary>
        /// Create an UnsuitableExportDefinitionIssue.
        /// </summary>
        /// <param name="reason">Reason code for the issue.</param>
        /// <param name="message">Description of the issue.</param>
        public UnsuitableExportDefinitionIssue(UnsuitableExportDefinitionReason reason, string message)
        {
            Reason = reason;
            Message = message;
        }
        
        /// <summary>
        /// Reason code for the issue.
        /// </summary>
        public UnsuitableExportDefinitionReason Reason { get; private set; }

        /// <summary>
        /// Description of the issue.
        /// </summary>
        public string Message { get; private set; }
    }
}
