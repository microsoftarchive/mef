## Catalogs
One of value propositions of MEF's attributed programming model is the ability to dynamically discover parts via catalogs. Catalogs allow applications to easily consume exports that have self-registered themselves via the Export attribute. Below is a list the catalogs MEF provides out of the box. 

### Assembly Catalog
To discover all the exports in a given assembly one would use the **{{[System.ComponentModel.Composition.Hosting.AssemblyCatalog](System.ComponentModel.Composition.Hosting.AssemblyCatalog)}}**. 

{Code:C#}
var catalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
{Code:C#}
{code:vb.net}
Dim catalog = New AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly())
{code:vb.net}

### Directory Catalog
To discover all the exports in all the assemblies in a directory one would use the **{{[System.ComponentModel.Composition.Hosting.DirectoryCatalog](System.ComponentModel.Composition.Hosting.DirectoryCatalog)}}**.

{Code:C#}
var catalog = new DirectoryCatalog("Extensions");
{Code:C#}
{code:vb.net}
Dim catalog = New DirectoryCatalog("Extensions")
{code:vb.net}

If a relative directory is used it is relative to the base directory of the current AppDomain.

The DirectoryCatalog will do a one-time scan of the directory and will not automatically refresh when there are changes in the directory. However, you can implement your own scanning mechanism, and call {{Refresh()}} on the catalog to have it rescan. Once it rescans, recomposition will occur.

{Code:C#}
var catalog = new DirectoryCatalog("Extensions");
//some scanning logic
catalog.Refresh();
{Code:C#}
{code:vb.net}
Dim catalog = New DirectoryCatalog("Extensions")
'some scanning logic
catalog.Refresh()
{code:vb.net}


**Note:** DirectoryCatalog is not supported in Silverlight.

### Aggregate Catalog
When AssemblyCatalog and DirectoryCatalog are not enough individually and a combination of catalogs is needed then an application can use an **{{[System.ComponentModel.Composition.Hosting.AggregateCatalog](System.ComponentModel.Composition.Hosting.AggregateCatalog)}}**. An AggregateCatalog combines multiple catalogs into a single catalog. A common pattern is to add the current executing assembly, as well as a directory catalog of third-party extensions. You can pass in a collection of catalogs to the AggregateCatalog constructor or you can add directly to the Catalogs collection i.e. catalog.Catalogs.Add(...)

{Code:C#}
var catalog = new AggregateCatalog(
  new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()), 
  new DirectoryCatalog("Extensions"));
{Code:C#}

### Type Catalog
To discover all the exports in a specific set of types one would use a **{{[System.ComponentModel.Composition.Hosting.TypeCatalog](System.ComponentModel.Composition.Hosting.TypeCatalog)}}**.

{Code:C#}
var catalog = new TypeCatalog(typeof(type1), typeof(type2), ...);
{Code:C#}
{code:vb.net}
Dim catalog = New TypeCatalog(GetType(type1), GetType(type2) , ...) 
{code:vb.net}

### DeploymentCatalog - Silverlight only
MEF in Silverlight includes the DeploymentCatalog for dynamically downloading remote XAPs. For more on that see the [DeploymentCatalog](DeploymentCatalog) topic.

### Using catalog with a Container
To use a catalog with the container, simpy pass the catalog to the container's constructor.

{Code:C#}
var container = new CompositionContainer(catalog);
{Code:C#}
{code:vb.net}
Dim container = New CompositionContainer(catalog) 
{code:vb.net}
