//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.IO;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Pretty printer for CompositionInfo objects.
    /// </summary>
    public static class CompositionInfoTextFormatter
    {
        /// <summary>
        /// Write a summary of info to output.
        /// </summary>
        /// <param name="info">Object to format.</param>
        /// <param name="output">Destination for formatted text.</param>
        public static void Write(CompositionInfo info, TextWriter output)
        {
            foreach (var part in info.PartDefinitions)
            {
                PartDefinitionInfoTextFormatter.Write(part, output);
                output.WriteLine();
            }
        }
    }
}
