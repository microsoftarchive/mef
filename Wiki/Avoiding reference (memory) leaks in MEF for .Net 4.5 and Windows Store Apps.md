All versions of MEF implement some level of tracking of the parts that the composition engine creates. In MEF for Windows Store apps (_System.Composition_ namespaces) this is for the purpose of ensuring that disposable parts can be released properly.

To avoid holding on to part instances for longer than is required, the following guidance applies.

Where multiple instances of a part need to be created, use an _ExportFactory<T>_ rather than calling _GetExports()_ multiple times on the container.

**This code may leak memory.**

{{
// Within the class declaration of a part
CompositionContext _container = ...;


// If page objects have disposable dependencies,
// they will be tracked by _container.
public Page CreatePage()
{
    _container.GetExport<Page>();
}
}}
**Creating a page: this code does not leak.**

{{
// Within the class declaration of a part

[Import](Import)
public ExportFactory<Page> PageFactory { get; set; }

public Page CreatePage()
{
   return PageFactory.CreateExport().Value;
}
}}
If the _Export<T>_ instance is garbage collected, all collectible parts will also be free for collection (regardless of whether or not _Dispose()_ is called).

The _Export<T>_ instance returned from the _CreateExport()_ method can be disposed in order to dispose related parts:

{{
public void ShowPage()
{
   using (var page = PageFactory.CreateExport())
   {
      // Outside the using block, the page and its dependencies
      // will be disposed.
      page.Value.Show();
   }
}
}}
If you need to use _CompositionContext_ (rather than _ExportFactory<T>_) for some reason, e.g. to implement a service locator, then use _ExportFactory<CompositionContext>_ to create a new _CompositionContext_ for each unit of work.

This applies when using _CompositionContext.SatisfyImports()_ as well.
