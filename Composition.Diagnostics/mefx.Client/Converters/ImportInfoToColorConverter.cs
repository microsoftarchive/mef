using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using mefx.Client.Models;
using System.ComponentModel.Composition.Primitives;
using Microsoft.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition;

namespace mefx.Client.Converters
{
	public class ImportInfoToColorConverter : IValueConverter
	{
		public Brush CardinalityErrorBrush { get; set; }

		public Brush NoUnsuitableExportsBrush { get; set; }

		public Brush ExportMatchingErrorBrush { get; set; }

		public Brush OptionalExportsUnsuitableBrush { get; set; }

		public Brush ProvidingPartRejectedBrush { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Brush brush = this.NoUnsuitableExportsBrush;
			
			ImportInfo importInfo = value as ImportInfo;

			if (importInfo != null)
			{
				if (importInfo.UnsuitableExportDefinitions.Any(
						uei => uei.Issues.Any(i => i.Reason != UnsuitableExportDefinitionReason.PartDefinitionIsRejected)))
				{
					brush = this.ExportMatchingErrorBrush;
				}
				else if (!importInfo.HasUnsuitableExportDefinitions)
				{
					if (importInfo.ActualException is ImportCardinalityMismatchException)
					{
						brush = CardinalityErrorBrush;
					}
					else
					{
						brush = this.NoUnsuitableExportsBrush;
					}
				}
				else if (importInfo.ImportCardinality == ImportCardinality.ZeroOrOne ||
						importInfo.ImportCardinality == ImportCardinality.ZeroOrMore)
				{
					brush = this.OptionalExportsUnsuitableBrush;
				}
				else if (true)
				{
					brush = this.ProvidingPartRejectedBrush;
				}
				
			}

			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
