# Frequently Asked Questions about Microsoft.Composition

## Where is {{SatisfyImports()}}?

{{CompositionContext.SatisfyImports()}} is an extension method. Make sure you have:

{{
using Microsoft.Composition;
}}
At the top of your source file.

## What about {{SatisfyImportsOnce()}}?

Some versions of MEF (e.g. on Silverlight) provide a {{SatisfyImportsOnce()}} method that allows imports on an existing object to be set, without registering that object for recomposition.

Microsoft.Composition does not support recomposition, and therefore {{SatisfyImports()}} has the appropriate behavior for this use case.

## How can I add existing instances to the composition?

Microsoft.Composition doesn't allow instances to be added directly to the container by default. Doing so makes tooling and testing harder, and programs more difficult to understand.

Instead, either:
* Create a part type and add it to the container
* Export the instance using a _property export_

If you find that there is no alternative design other than adding the instance directly, you can use the approach found in the _Microsoft.Composition.Demos.ExtendedPartTypes_ sample project to add instances using an extension.

## Why am I having trouble getting {{[ImportMany](ImportMany)}} to work?

Make sure you're not setting the {{ContractType}} attribute parameter unnecessarily; its behavior changed from earlier versions of MEF.

Instead of:

{{
[ImportMany(typeof(IFoo))](ImportMany(typeof(IFoo)))
public IEnumerable<IFoo> Foos { get; set; }
}}
Use:

{{
[ImportMany](ImportMany)
public IEnumerable<IFoo> Foos { get; set; }
}}

Also, the collection types supported by Microsoft.Composition are fewer than earlier MEF versions. The only types supported for {{[ImportMany](ImportMany)}} are:
* IEnumerable<T>
* ICollection<T>
* IList<T>
* T[]()

## My import isn't being set, what could be wrong?

* Is the member (property or constructor) being imported public?
* Field imports aren't supported, use properties instead
* Is the imported member the exact same contract type as the export?

## Why can't I import a metadata view?

Interface-based metadata views are not supported in Microsoft.Composition. Instead of interfaces, Microsoft.Composition uses classes with auto-properties to implement metadata views, see the [Migration Guide](MetroChanges).

## How can I use existing System.ComponentModel.Composition parts?

Full-framework MEF parts aren't supported out of the box, but we provide a [Catalog Hosting](http://mef.codeplex.com/SourceControl/changeset/view/703624739918#oob%2fdemo%2fMicrosoft.Composition.Demos.CatalogHosting%2fCatalogExportDescriptorProvider.cs) sample showing how to do this. You can add a {{CatalogExportDescriptorProvider}} to your {{ContainerConfiguration}} using the {{WithProvider()}} method.