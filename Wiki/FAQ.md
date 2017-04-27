# Using CompositionContainer

## How can I get Exports using a Type object?

CompositionContainer does not provide non-generic overloads of many of the GetExport* methods, e.g. GetExportedValue().

{code:c#}
// not supported
object value = container.GetExportedValue(type);
{code:c#}
{code:vb.net}
' not supported
Dim value As Object = container.GetExportedValue(type) 
{code:vb.net}

Instead, use the GetExports(ImportDefinition) overload:

{code:c#}
IEnumerable<Export> matching = container.GetExports(

  new ContractBasedImportDefinition(

    AttributedModelServices.GetContractName(type),

    AttributedModelServices.GetTypeIdentity(type),

    Enumerable.Empty<string>(),

    ImportCardinality.ExactlyOne,

    false,

    false,

    CreationPolicy.Any));

object value = matching.Single().Value;
{code:c#}
{code:vb.net}
Dim matching As IEnumerable(Of Export) = container.GetExports(
    New ContractBasedImportDefinition(
        AttributedModelServices.GetContractName(Type), 
        AttributedModelServices.GetTypeIdentity(Type), 
        Enumerable.Empty(Of String)(), 
        ImportCardinality.ExactlyOne, 
        False, 
        False, 
        CreationPolicy.Any)) 
Dim value As Object = matching.Single().Value
{code:vb.net}

## How do I use Nested Containers?

Nested containers are sometimes used as a convenient way to manage lifetime and sharing scope.

The best available source of information discusses filtered catalogs: [Filtering Catalogs](Filtering-Catalogs).

When child containers are created, they're typically given two constructor parameters - their own catalog, and their parent (a CompositionContainer passed as an ExportProvider.)

Using this configuration, all dependencies are requested via the most-nested child container.

If a dependency can be satisfied from the container's own catalog, it will do so. The lifetime of any resulting part will be tied to the lifetime of the child container, and all of that part's dependencies will be satisfied from the same container (recursively, using this algorithm.)

When a child container can't satisfy a dependency via its own catalog, it calls into its parent. If a dependency is retrieved from the parent this way, the lifetime of the resulting part will be tied to the parent. Any dependencies that part has will also be satisfied by the parent, never calling back into the child.

The key to setting up the whole configuration successfully is the catalog provided to each container.

As a general rule, any part that is NonShared should be able to be created at any level, and so should appear in all the containers' catlogs.

In a hierarchy of containers, Shared parts are a bit trickier. Sharing is related to the container in which the part is created, so a Shared part in a child container's catalog will be shared within that specific child container; other containers that can see the same part in their catalogs will create and share their own instances.

This means that within a single object graph, it may be possible to navigate to more than one instance of a Shared part (e.g. one in the child, another accessible through a dependency resolved in the parent.)
