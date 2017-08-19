namespace mefx.Client.Models
{
    using System.ComponentModel.Composition.Primitives;
    using Microsoft.ComponentModel.Composition.Diagnostics;

    public class ExportInfo
    {
        private ExportDefinition _exportDefinition;

        public ExportInfo(ExportDefinition exportDefinition)
        {
            this._exportDefinition = exportDefinition;
        }

        public string DisplayName
        {
            get
            {
                return CompositionElementTextFormatter.DisplayCompositionElement(this._exportDefinition);
            }
        }
    }
}