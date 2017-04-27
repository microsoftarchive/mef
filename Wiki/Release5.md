# MEF Preview 5 Release Notes

## Namespace changes
* System.ComponentModel.Composition – For part authors.  This namespace contains the import and export attributes as well as other APIs that part authors are likely to use.
*  System.ComponentModel.Composition.Hosting – For hosters.  This namespace contains the CompositionContainer, catalog implementations, and other APIs that hosters are likely to use.
## Part discovery changes
* CompositionOptionsAttribute and DiscoveryMode have been removed.
* PartNotDiscoverableAttribute replaces {"[CompositionOptions(DiscoveryMode = DiscoveryMode.Never)](CompositionOptions(DiscoveryMode-=-DiscoveryMode.Never))"}
* PartCreationPolicyAttribute can be used to specify part creation policy (Replaces CompositionOptionsAttribute.CreationPolicy)
* By default, exports on a base class will not be included in the exports for a derived class.
* The PartExportsInheritedAttribute can be applied to a base class to specify that exports on that class will be included in derived classes. **NOTE**: If this attribute is used, and both the base and derived class have export attributes applied to them, there will be +multiple exports+ created for the same class.  Also note that whatever metadata is applied to the export on the base class can’t be added to or overridden by the derived class
## Collection imports
* ImportManyAttribute should now be used on collection imports (in the future, an ImportAttribute will not be interpreted as a collection import even if it is applied to a collection type).
* Array imports are now supported.
## Typed Imports/Exports
* Imports and Exports now match on type as well as contract.
* Exporters of string contracts such  as {"[Export(“Foo”)](Export(“Foo”))"} must now specify the type they expect to be imported as well. e.g. {"[Export(“Foo”, typeof(string))](Export(“Foo”,-typeof(string)))"}
## Method exports
* Method exports can now be imported as custom delegates in addition to Action<…> and Func<…> delegates
## Directory Catalog
* The directory watching functionality has been removed.  The Refresh() method has been added to explicitly update the catalog with new assemblies in the directory.
## Removal of Caching / new infrastructure
* The old caching infrastructure has been removed.  We’ve added a general purpose API that allows the implementation of catalog caching as well as supporting builders of custom programming models.  The APIs are members of the static class System.ComponentModel.Composition.ReflectionModel.ReflectionModelServices.  In the future we plan to ship a sample that shows how these APIs can be used to create cached catalogs.
## New sample application
* MEF Studio – a designer hosting sample.
## Common compilation errors to expect when migrating previous code bases
* System.ComponentModel.Composition.Container does not exist -> Add reference to System.ComponentModel.Composition.Hosting namespace
* CompositionContainer does not contain method AddPart or Compose -> Need to start using CompositionBatch, or one of the helper extension methods ComposeParts or ComposeExportedObjects
* CompositionOptionsAttribute does not exist -> For CreationPolicy use PartCreationPolicyAttribute
* INotifyImportCompleted does not exist -> Use IPartImportsSatisfiedNotification interface and change method from ImportCompleted to OnImportsSatisfied
