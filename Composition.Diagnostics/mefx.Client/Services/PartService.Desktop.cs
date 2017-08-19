namespace mefx.Client.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Microsoft.Win32;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.Hosting;

	public partial class PartService
	{
        public void PromptForParts(Action<IEnumerable<ComposablePartCatalog>> callback)
		{

			if (callback != null)
			{
                List<ComposablePartCatalog> files = new List<ComposablePartCatalog>();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Assemblies (*.dll;*.exe)|*.dll;*.exe";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == true)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        files.Add(new AssemblyCatalog(new FileInfo(file).ToAssembly()));
                    }
                }

                callback(files);
			}
		}
	}
}
