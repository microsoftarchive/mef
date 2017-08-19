namespace mefx.Client.Models
{
	using System.ComponentModel.Composition;

    [Export(typeof(ShellViewModel))]
	public sealed class ShellViewModel : NotifyObject
	{
	}
}