
namespace mefx.Client
{
	using System.Windows;
	using System.ComponentModel.Composition;
	using mefx.Client.Models;

	public partial class Shell : Window
	{
		public Shell()
		{
			InitializeComponent();

			CompositionInitializer.SatisfyImports(this);
		}

		[Import]
		public ShellViewModel ViewModel
		{
			set
			{
				this.DataContext = value;
			}
		}
	}
}