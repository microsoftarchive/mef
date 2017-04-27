# Overriding the host configuration
MEF creates a default host configuration for {{CompositionInitializer}} the first time {{SatisfyImports}} is called. This is ideal either for getting off the ground with MEF, or for simple applications which simply discover all parts in their current XAP. For more complex scenarios such as [application partitioning](DeploymentCatalog) we offer the {{System.ComponentModel.Composition.Hosting.CompositionHost}} class (in System.ComponentModel.Composition.Initialization.dll) which allows you to override the configuration.  To use {{CompositionHost}} you call its initialize method in your app startup and specify your own configuration which MEF will use.  

**Note**: CompositionHost.Initialize can only be called a single time within the app. Future calls with throw an exception. We also recommend that only the host calls this code rather than 3rd party parts.
# Overriding with catalogs
The simplest way to override the config is to call the overload of Initialize which accepts catalogs. MEF then creates a container behind the scenes that uses those catalogs. For example below I am overriding the config to pass in an AggregateCatalog which I can use to add other catalogs in the future.

{code:c#}
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

public class App : Application {
  public App() {
    this.Startup += this.Application_Startup;
    this.Exit += this.Application_Exit;
    this.UnhandledException += this.Application_UnhandledException;

    InitializeComponent();
  }

  private void Application_Startup(object sender, StartupEventArgs e)
  {
    var aggregateCatalog = new AggregateCatalog();
    var assemblyCatalog = new AssemblyCatalog(typeof(App).Assembly);
    CompositionHost.Initialize(assemblyCatalog, aggregateCatalog);
    CompositionInitializer.SatisfyImports(this);  //imports are satisfied
    ...
   }

   [Import](Import)
   public MainViewModel ViewModel {get;set;}
{code:c#}
{code:vb.net}
Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting

Public Class App
    Inherits Application
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        Dim aggregateCatalog_Renamed = New AggregateCatalog()
        Dim assemblyCatalog_Renamed = New AssemblyCatalog(GetType(App).Assembly) 
        CompositionHost.Initialize(assemblyCatalog_Renamed, aggregateCatalog_Renamed) 
        CompositionInitializer.SatisfyImports(Me) 'imports are satisfied
       ...
    End Sub

    <Import()>
    Public Property ViewModel() As MainViewModel
{code:vb.net}

In the code above the application is calling Initialize passing in an AggregateCatalog and an assembly catalog for the current assembly. SatisfyImports is then called which causes the ViewModel to get imported.
## Configuring to discover parts in the current XAP.
When you override you take full control, MEF does not assume anything about the config. This means the parts in the main XAP are not discovered. For example above we passed in an assembly catalog with the current assembly. If you want the main XAP to be discovered you can pass in a {{DeploymentCatalog}} created with the default constructor. This tells MEF "find all parts in the current XAP". Below is how our startup code looks if we use this.
{code:c#}
  private void Application_Startup(object sender, StartupEventArgs e)
  {
    var aggregateCatalog = new AggregateCatalog();
    CompositionHost.Initialize(new DeploymentCatalog(),aggregateCatalog); //add the current XAP.
    CompositionInitializer.SatisfyImports(this);  //imports are satisfied
    ...
   }
{code:c#}
{code:vb.net}
Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
    Dim aggregateCatalog_Renamed = New AggregateCatalog()
    CompositionHost.Initialize(New DeploymentCatalog(), aggregateCatalog_Renamed) 'add the current XAP. 
    CompositionInitializer.SatisfyImports(Me) 'imports are satisfied
   ... 
End Sub
{code:vb.net}

# Overriding with a container
In general overrding with catalogs should be sufficient. For more advanced scenarios such as providing a scoped container strategy you may want to override the container itself. To do this you can call the override of {{Initialize}} that accepts a container. Below is how the startup code above changes if we do that.

{code:c#}
  private void Application_Startup(object sender, StartupEventArgs e)
  {
    var aggregateCatalog = new AggregateCatalog();
    var container = new CompositionContainer(new DeploymentCatalog(), aggregateCatalog);
    CompositionHost.Initialize(container);
    CompositionInitializer.SatisfyImports(this);  //imports are satisfied
    ...
   }
{code:c#}
{code:vb.net}
    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        Dim aggregateCatalog_Renamed = New AggregateCatalog()
        Dim container = New CompositionContainer(New DeploymentCatalog(), aggregateCatalog_Renamed) 
        CompositionHost.Initialize(container) 
        CompositionInitializer.SatisfyImports(Me) 'imports are satisfied
   ... 
    End Sub
{code:vb.net}
