namespace mefx.Client.Services
{
    using System.ComponentModel.Composition.Primitives;

    public interface ICatalogService
    {
        void AddCatalog(ComposablePartCatalog catalog);
    }
}
