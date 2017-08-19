using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Pretty printer for PartDefinitionInfo. Currently not very 'pretty' but should
    /// be friendly for log file searching.
    /// </summary>
    public static class PartDefinitionInfoTextFormatter
    {
        /// <summary>
        /// Write details of a part definition to a text writer.
        /// </summary>
        /// <param name="part">Part definition to print.</param>
        /// <param name="output">Destination.</param>
        public static void Write(PartDefinitionInfo part, System.IO.TextWriter output)
        {
            output.WriteLine("[Part] {0}", CompositionElementTextFormatter.DescribeCompositionElement(part.PartDefinition));

            if (part.IsPrimaryRejection)
                output.WriteLine("  [Primary Rejection]");

            foreach (var ed in part.PartDefinition.ExportDefinitions)
                output.WriteLine("  [Export] {0}", CompositionElementTextFormatter.DisplayCompositionElement(ed));

            foreach (var id in part.ImportDefinitions)
            {
                output.WriteLine("  [Import] {0}", CompositionElementTextFormatter.DisplayCompositionElement(id.ImportDefinition));
                foreach (var e in id.Actuals)
                {
                    output.WriteLine("    [SatisfiedBy] {0}", CompositionElementTextFormatter.DescribeCompositionElement(e));
                }

                if (id.Exception != null)
                    output.WriteLine("    [Exception] {0}", id.Exception);

                if (!(id.ImportDefinition is ContractBasedImportDefinition))
                    output.WriteLine("    [Not Contract-Based]");
                else
                    foreach (var potential in id.UnsuitableExportDefinitions)
                    {
                        output.WriteLine("    [Unsuitable] {0}", CompositionElementTextFormatter.DescribeCompositionElement(potential.ExportDefinition));
                        foreach (var issue in potential.Issues)
                            output.WriteLine("      [Because] {0}, {1}", issue.Reason, issue.Message);
                    }
            }
        }
    }
}
