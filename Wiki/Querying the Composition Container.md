# Querying the CompositionContainer

The {{CompositionContainer}} exposes a handful of overloads to get exports and exported objects and collections of both.

You need to observe the following behavior shared among these methods overloads – unless noted otherwise. 
* When requesting a single instance, an exception will be thrown if none is found
* When requesting a single instance, an exception will be thrown if more than one is found

## GetExportedValue
In the following code snippet we request an instance of Root (contract) instance.
{code:c#}
var container = new CompositionContainer(new AssemblyCatalog(typeof(Program).Assembly));
Root partInstance = container.GetExportedValue<Root>();
{code:c#}
{code:vb.net}
Dim container = New CompositionContainer(New AssemblyCatalog(GetType(Program).Assembly)) 
Dim partInstance As Root = container.GetExportedValue(Of Root)() 
{code:vb.net}

If you have an export under a different contract name, you will need to use a different overload:

{code:c#}
[Export("my_contract_name")](Export(_my_contract_name_))
public class Root
{
}

var container = new CompositionContainer(new AssemblyCatalog(typeof(Program).Assembly));
Root partInstance = container.GetExportedValue<Root>("my_contract_name");
{code:c#}
{code:vb.net}
<Export("my_contract_name")>
Public Class Root
End Class

Private container = New CompositionContainer(New AssemblyCatalog(GetType(Program).Assembly)) 
Private partInstance As Root = container.GetExportedValue(Of Root)("my_contract_name")
{code:vb.net}

## GetExport
{{GetExport}} retrieves a lazily instantiated reference to an export. Accessing the {{Value}} property of the export will either force the export instance to be created. Successive invocation of the export's Value will return the same instance, regardless of whether the part has a Shared or Non-Shared lifetime.

{code:c#}
Lazy<Root> export = container.GetExport<Root>();
var root = export.Value; //create the instance.
{code:c#}
{code:vb.net}
Dim export As Lazy(Of Root) = container.GetExport(Of Root)() 
Dim root = export.Value 'create the instance. 
{code:vb.net}

## GetExportedValueOrDefault

{{GetExportedValueOrDefault}} works exactly as {{GetExportedValue}} with the difference that it won’t throw an exception in case nothing matches. 

{code:c#}
var root = container.GetExportedValueOrDefault<Root>(); // may return null
{code:c#}

{code:vb.net}
Dim root = container.GetExportedValueOrDefault(Of Root)() ' may return null
{code:vb.net}
