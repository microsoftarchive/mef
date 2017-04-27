# Dynamic Instantiation

A [Lazy Import](Lazy-Exports) provides an importer with exactly one instance of the exported contract. The underlying value is created on first use with subsequent requests returning the same instance.

{code:c#}
[Import](Import)
Lazy<IFoo> foo;

// elsewhere in the same class

Assert.AreSame(foo.Value, foo.Value);
{code:c#}
{code:vb.net}
<Import()>
Private foo As Lazy(Of IFoo) 

' elsewhere in the same class

Assert.AreSame(foo.Value, foo.Value) 
{code:vb.net}

## ExportFactory<T>

Sometimes, a part needs to create multiple non-shared instances of an export on-the-fly. For example an order management application needs to create new OrderViewModels on the fly as a new order is created. For these situations, ExportFactory<T>, rather than Lazy<T>, is the appropriate type to import:

{code:c#}
[Export](Export)
public class OrderController {

  [Import](Import) 
  public ExportFactory<OrderViewModel> OrderVMFactory {get;set;}

  public OrderViewModel CreateOrder() {
    return OrderVMFactory.CreateExport().Value;
  }
}
{code:c#}
{code:vb.net}
<Export()>
Public Class OrderController

    <Import()>
    Public Property OrderVMFactory() As ExportFactory(Of OrderViewModel) 

    Public Function CreateOrder() As OrderViewModel
        Return OrderVMFactory.CreateExport().Value
    End Function
End Class
{code:vb.net}

The OrderController above imports a factory for creating OrderViewModels. Each type the application calls CreateOrder, the controller will use the factory to manufacture a new ViewModel instance. That ViewModel is created by the underlying container which will provide it's imports.

Importing a ExportCreator<T> is very similar to an import of Lazy<T>. The same rules for determining contract names and types apply, and the ExportCreator<T, TMetadata> type presents strongly-typed metadata in the same way as Lazy<T, TMetadata>.

## ExportLifetimeContext<T>

The return value of ExportCreator<T>.CreateExport() is a ExportLifetimeContext<T>. This type exposes the created part's exported value through the Value property. The type also provides an implementation of IDisposable, so that when the exported value is no longer required, the underlying part and all of its dependencies can be cleaned up.

More details can be found in [this blog post](http://blogs.msdn.com/nblumhardt/archive/2009/08/28/dynamic-part-instantiation-in-mef.aspx).