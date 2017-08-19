namespace mefx.Client
{
	using System.Windows;

	internal sealed class Bootstrapper
	{
		public void Run()
		{
			this.CreateShell();
		}

		private void CreateShell()
		{
			Shell shell = new Shell();

			Application.Current.RootVisual = shell;
		}
	}
}
