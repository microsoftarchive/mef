namespace mefx.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Windows.Controls;
    using mefx.Client.Helpers;

	public partial class PartService
	{
		public void PromptForParts(Action<IEnumerable<ComposablePartCatalog>> callback)
		{
			if (callback != null)
			{
				List<ComposablePartCatalog> catalogs = new List<ComposablePartCatalog>();

				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "XAPs (*.xap)|*.xap";
				dialog.Multiselect = false;

				if (dialog.ShowDialog() == true)
				{
                    var fileStream = dialog.File.OpenRead();

                    var assemblies = Package.LoadPackagedAssemblies(fileStream);

                    AggregateCatalog catalog = new AggregateCatalog();

                    CatalogHelper.DiscoverParts(catalog, assemblies);

                    catalogs.Add(catalog);
				}

                callback(catalogs);
			}
		}
	}
}