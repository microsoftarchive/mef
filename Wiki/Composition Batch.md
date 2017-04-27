# Composition Batch

A MEF container instance is not immutable. Changes can happen if the catalog supports changes (like watching a directory for changes) or if your code add or remove parts in runtime. Previously you had to make the changes and invoke the Compose method on the CompositionContainer. In the Preview 4 release we have introduced support for a composition batch. 

The batch contains a list of parts to be added and/or removed. After performing the changes, the container automatically triggers a composition which updates recomposable imports affected by the changes.
As a scenario, consider a settings window and a user selecting and deselecting options. Those would map to parts present or not on the container. To apply the batch, you would call the Compose method, as follows:

{code:c#}
var batch = new CompositionBatch();
batch.AddPart(partInstance1);
batch.AddPart(partInstance2);
batch.RemovePart(part3);

container.Compose(batch);
{code:c#}
{code:vb.net}
Dim batch = New CompositionBatch()
batch.AddPart(partInstance1) 
batch.AddPart(partInstance2) 
batch.RemovePart(part3) 
container.Compose(batch) 
{code:vb.net}

For types that actually use the attributed programming model there are some extension methods on AttributedModelServices for CompositionContainer that allow you to do hide the CompositionBatch in some common cases where it isn't needed.
{code:c#}
container.ComposeParts(partInstance1, partInstance2,... ); // creates a CompositionBatch and calls AddPart on all the passed parts followed by Compose
container.ComposeExportedValue<IFoo>(instanceOfIFoo); // creates a CompositionBatch and calls AddExportedValue<T> followed by Compose.
{code:c#}
{code:vb.net}
container.ComposeParts(partInstance1, partInstance2,...) ' creates a CompositionBatch and calls AddPart on all the passed parts followed by Compose
container.ComposeExportedValue(Of IFoo)(instanceOfIFoo) ' creates a CompositionBatch and calls AddExportedValue<T> followed by Compose. 
{code:vb.net}
