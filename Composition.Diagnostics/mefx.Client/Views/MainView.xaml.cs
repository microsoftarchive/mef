namespace mefx.Client.Views
{
	using System.ComponentModel.Composition;
	using System.Windows.Controls;
	using mefx.Client.Models;
	using System.Windows.Data;
	using System.Windows;
	using System;

	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();

            CompositionInitializer.SatisfyImports(this);
		}

		[Import]
		public MainViewModel ViewModel
		{
			set
			{
				value.ShowPart = (item => SetSelectedItem(item));
				this.DataContext = value;				
			}
		}

		public void SetSelectedItem(object selectedItem)
		{
			listBox.SelectedItem = selectedItem;
		}

		//private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		//{
		//    ((MainViewModel)DataContext).OpenFiles();
		//}
	}

	public class MyGridSplitter : GridSplitter
	{
		
	}

    //  Built-in BooleanToVisibilityConverter doesn't exist on Silverlight
	public sealed class BooleanVisibilityConverter : IValueConverter
	{
		// Methods
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool flag = false;
			if (value is bool)
			{
				flag = (bool)value;
			}
			else if (value is bool?)
			{
				bool? nullable = (bool?)value;
				flag = nullable.HasValue ? nullable.Value : false;
			}
			return (flag ? Visibility.Visible : Visibility.Collapsed);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
		}
	}
}