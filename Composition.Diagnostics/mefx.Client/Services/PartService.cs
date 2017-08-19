namespace mefx.Client.Services
{
	using System;
	using System.IO;
	using System.ComponentModel.Composition;

	[Export(typeof(IPartService))]
	public partial class PartService : IPartService
	{
	}
}