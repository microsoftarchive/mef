namespace mefx.Client.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using mefx.Client.Models;

    public class PartInfoToColorConverter : IValueConverter
    {
        public Brush NormalBrush { get; set; }

        public Brush IsRejectedBrush { get; set; }

        public Brush IsPrimaryRejectionBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = this.NormalBrush;

            PartInfo partInfo = value as PartInfo;

            if (partInfo != null && partInfo.IsRejected && partInfo.IsPrimaryRejection)
            {
                brush = this.IsPrimaryRejectionBrush;
            }
            else if (partInfo != null && partInfo.IsRejected)
            {
                brush = this.IsRejectedBrush;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}