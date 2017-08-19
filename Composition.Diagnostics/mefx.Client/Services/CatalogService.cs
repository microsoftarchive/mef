namespace mefx.Client.Services
{
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;

    public class CatalogService : ICatalogService
    {
        private AggregateCatalog _catalog;

        public CatalogService(AggregateCatalog catalog)
        {
            this._catalog = catalog;
        }

        public void AddCatalog(ComposablePartCatalog catalog)
        {
            this._catalog.Catalogs.Add(catalog);
        }
    }
}
