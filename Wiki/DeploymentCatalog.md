# Paritioning applications across multiple XAPs
The default Silverlight programming model requires all MEF parts to be in the current XAP. This is fine for simple Silverlight composable applications. It poses severe problems for very large applications:
# Causes the default XAP to continually bloat which increases the initial app download and hinders the user's startup experience. 
# Prevents Silverlight applications from offering an extensibility experience similar to the desktop. In many scenarios it is ideal to allow applications to be extended _on the server_.
# For multiple teams on a large app it makes it difficult to add new functionality to the application.
MEF offers a solution to both through a new catalog called {{System.ComponentModel.Composition.Hosting.DeploymentCatalog}} located in the Initialization dll.

# DeploymentCatalog
{{DeploymentCatalog}} downloads parts from XAPs living on the web server. Using it you can break your application into as many XAPs as you like and use it to bring them back together. {{DeploymentCatalog}} downloads catalogs asynchronously and implements the async event pattern in order to allow you to monitor the download i.e. track completion, failures, etc. Because DeploymentCatalog is recomposable hower it is not _required_ for you to do this, but we recommend you do. To use {{DeploymentCatalog}} with {{CompositionInitializer}} you must override the default configuration using {{CompositionHost}} otherwise {{CompositionInitializer}} will never see the downloaded parts.

# Downloading parts lazily once the app starts.
The most common (and simplest) scenario for using DeploymentCatalog is to reduce your app startup footprint and immediately start downloading other pieces in the background. For example below (hardcoding of modules is for illustration but not necessary) the order management system starts up with only the Home and Order modules present (not shownin the code). It immediately initiates download of the "Admin", "Reporting" and "Forecasting" modules which are not required initially. [Importing a ViewModel dropped for simplicity](Importing-a-ViewModel-dropped-for-simplicity). Below is a diagram that indicates the design.

[Image:Partitioned App.png)(Image_Partitioned-App.png)

Here is a code snippet that shows how this will work.

{code:c#}
public class App : Application {
  public App() {
    this.Startup += this.Application_Startup;
    this.Exit += this.Application_Exit;
    this.UnhandledException += this.Application_UnhandledException;

    InitializeComponent();
  }

  private void Application_Startup(object sender, StartupEventArgs e)
  {
    var catalog = new AggregateCatalog();
    catalog.Catalogs.Add(CreateCatalog("Modules/Admin"));
    catalog.Catalogs.Add(CreateCatalog("Modules/Reporting"));
    catalog.Catalogs.Add(CreateCatalog("Modules/Forecasting"));

    CompositionHost.Initialize(new DeploymentCatalog(), catalog);
    CompositionInitializer.SatisfyImports(this)

    RootVisual = new MainPage();
  }

  private DeploymentCatalog CreateCatalog(string uri) {
    var catalog = new DeploymentCatalog(uri);
    catalog.DownloadCompleted += (s,e) => DownloadCompleted();
    catalog.DownloadAsync();
    return catalog;
  }

  private void DownloadCompleted(object sender, AsyncCompletedEventArgs e) {
    if (e.Error != null) {
      MessageBox.Show(e.Error.Message);
    }
  }

  private Lazy<IModule, IModuleMetadata>[]() _modules;

  [ImportMany(AllowRecomposition=true))](ImportMany(AllowRecomposition=true)))
  public Lazy<IModule, IModuleMetadata>[]() Modules {
    get{return _modules;}
    set{
      _modules = value;
      ShowModules()
  }

  private void ShowModules() {
    //logic to show the modules
  }

}
{code:c#}
{code:vb.net}
Public Class App
    Inherits Application

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal sender As Object, ByVal e As StartupEventArgs) 
        Dim catalog = New AggregateCatalog()
        catalog.Catalogs.Add(CreateCatalog("Modules/Admin"))
        catalog.Catalogs.Add(CreateCatalog("Modules/Reporting"))
        catalog.Catalogs.Add(CreateCatalog("Modules/Forecasting"))

        CompositionHost.Initialize(New DeploymentCatalog(), catalog) 
        CompositionInitializer.SatisfyImports(Me) 
        RootVisual = New MainPage()
    End Sub


    Private Function CreateCatalog(ByVal uri As String) As DeploymentCatalog
        Dim catalog = New DeploymentCatalog(uri) 
        AddHandler catalog.DownloadCompleted, Sub(s, e) DownloadCompleted()
        catalog.DownloadAsync()
        Return catalog
    End Function

    Private Sub DownloadCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs) 
        If e.Error IsNot Nothing Then
            MessageBox.Show(e.Error.Message) 
        End If
    End Sub

    Private _modules() As Lazy(Of IModule, IModuleMetadata) 

    <ImportMany(AllowRecomposition:=True)> 
  Public Property Modules() As Lazy(Of IModule, IModuleMetadata)() 
        Get
            Return _modules
        End Get
        Set(ByVal value As Lazy(Of IModule, IModuleMetadata)()) 
            _modules = value
            ShowModules()
        End Set

    End Property

    Private Sub ShowModules()
        'logic to show the modules
End Sub

End Class
{code:vb.net}

There's a bunch of things going on above to make this work. First we are creating an {{AggregateCatalog}} which we are populating with 3 {{DeploymentCatalogs}} for our modules by calling a {{CreateCatalog}} helper method. CreateCatalog does the following:
* Creates a new {{DeploymentCatalog passing}} in a relative uri. In this case "Modules/" will be off off of "ClientBin/" on the server.
* Subscribes to the {{DownloadCompleted event}}. This is necessary primarily to track any failures that occur during the download.
* Starts the async download
* Returns the catalog
Next {{CompositionHost.Initialize}} is called passing in an empty DeploymentCatalog (in order to get parts in the current XAP) and the aggregate catalog which contains the DeploymentCatalogs being downloaded. Finally the app composes. The app has a single imports of all available modules which at startup will be minimally those modules included in the  main XAP. Once the modules are imported, {ShowModules} is called to display them. Notice the app does not wait for the download to complete rather it composed immediately with whatever parts are present. Also notice that in the DownloadCompleted event we are not having to handle adding the new parts to the container. More on this in the next section.
# Composing dynamically downloaded parts and using Recomposition.
In the code snippet above we did not wait for the new parts to show up, nor did we compose the app. This raises the question of how the new modules will show up. The answer is that the app relies on a feature of MEF called Recomposition. The modules import is marked with {{AllowRecomposition=true}}. This tells MEF that new parts are allowed to show up after the initial composition. DeploymentCatalog is recomposable meaning after the download of the XAP completes and it discovers parts, it will automatically tell MEF new parts are there. When that happens the Modules import will automatically be replaced with new newer set of modules. It will not replace the existing module instances. Thus DeploymentCatalog and Recomposition complement each other for application partitioning scenarios.

**Note:** Recomposition requires that existing imports of those contracts are recomposable, otherwise MEF prevents the recomposition which will result in an exception when adding a DeploymentCatalog. For example if {{AllowRecomposition = true}} is removed from Modules, then MEF will throw an exception when new modules show up.

# Downloading parts on demand after startup based on user action
The previous topics discussed downloading XAPs at startup. An alternative scenario is an app which downloads parts on-demand after the app is running. In this case it is not uncommon to have other parts in the system have access to downloading new XAPs. For example in stead of downloading the above modules automatically in the previous OrderManagement scenario, you can have modules _only_ download if needed.

In order to setup this type of configuration a bit more host setup work is required. The apis you will use are the same, with one addition. Instead of simply passing an aggregate catalog at the time of construction, you will wrap the catalog in a service which will be exported to other parts thus allowing _them_ to pull down new XAPs. Below is sample code that illustrates how to do this through the help of a DeploymentCatalogService.

{code:c#}
using System.ComponentModel;
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
    DeploymentCatalogService.Initialize();
    CompositionInitializer.SatisfyImports(this);

    var mainPage = new MainPage();
    mainPage.DataContext = ViewModel;
    RootVisual = mainPage;
  }

  [Export(typeof(IDeploymentCatalogService))](Export(typeof(IDeploymentCatalogService)))
  public class DeploymentCatalogService : IDeploymentCatalogService
  {
    private static AggregateCatalog _aggregateCatalog;

    Dictionary<string, DeploymentCatalog> _catalogs;

    public DeploymentCatalogService()
    {
      _catalogs = new Dictionary<string, DeploymentCatalog>();
    }

    public static void Initialize()
    {
      _aggregateCatalog = new AggregateCatalog();
      _aggregateCatalog.Catalogs.Add(new DeploymentCatalog());
       CompositionHost.Initialize(_aggregateCatalog);
    }

    public void AddXap(string relativeUri, Action<AsyncCompletedEventArgs> completedAction)
    {
      DeploymentCatalog catalog;
      if (!_catalogs.TryGetValue(uri, out catalog))
      {
        catalog = new DeploymentCatalog(uri);

        if (completedAction != null)
          catalog.DownloadCompleted += (s, e) => completedAction(e);
        else
          catalog.DownloadCompleted += (s,e) => DownloadCompleted;

        catalog.DownloadAsync();
        _catalogs[uri](uri) = catalog;
        _aggregateCatalog.Catalogs.Add(catalog);
      }

      void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
      {
        if (e.Error != null)
        {
          throw new InvalidOperationException(e.Error.Message, e.Error);
        }
      }
    }
    
  }
  
  public interface IDeploymentService {
    AddXap(string relativeUri, Action<AsyncCompletedEventArgs> completedAction);
  }
{code:c#}
{code:vb.net}
Imports System.ComponentModel
Imports System.ComponentModel.Composition.Hosting

Public Class App
    Inherits Application
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        DeploymentCatalogService.Initialize()
        CompositionInitializer.SatisfyImports(Me) 
        Dim mainPage_Renamed = New MainPage()
        mainPage_Renamed.DataContext = ViewModel
        RootVisual = mainPage_Renamed
    End Sub


    <Export(GetType(IDeploymentCatalogService))> 
    Public Class DeploymentCatalogService
        Implements IDeploymentCatalogService
        Private Shared _aggregateCatalog As AggregateCatalog
        Private _catalogs As Dictionary(Of String, DeploymentCatalog) 

        Public Sub New()
            _catalogs = New Dictionary(Of String, DeploymentCatalog)() 
        End Sub

        Public Shared Sub Initialize()
            _aggregateCatalog = New AggregateCatalog()
            _aggregateCatalog.Catalogs.Add(New DeploymentCatalog())
            CompositionHost.Initialize(_aggregateCatalog) 
        End Sub

        Public Sub AddXap(ByVal Uri As String, ByVal completedAction As Action(Of AsyncCompletedEventArgs)) Implements IDeploymentService.AddXap
            Dim catalog As DeploymentCatalog
            If Not _catalogs.TryGetValue(Uri, catalog) Then
                catalog = New DeploymentCatalog(Uri) 
                If completedAction IsNot Nothing Then
                    AddHandler catalog.DownloadCompleted, Sub(s, e) completedAction(e) 
                Else
                    AddHandler catalog.DownloadCompleted, Sub(s, e) DownloadCompleted()
                End If
                catalog.DownloadAsync()
                _catalogs(Uri) = catalog
                _aggregateCatalog.Catalogs.Add(catalog) 
            End If
        End Sub

        Private Sub DownloadCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs) 
            If e.Error IsNot Nothing Then
                Throw New InvalidOperationException(e.Error.Message, e.Error) 
            End If
        End Sub

    End Class
End Class


Public Interface IDeploymentService
    Sub AddXap(ByVal relativeUri As String, ByVal completedAction As Action(Of AsyncCompletedEventArgs)) 
End Interface
{code:vb.net}

In the sample above, a {{DeploymentCatalogService}} is introduced. It exposes a static {{Initialize}} method which is used to initializes the {{CompositionHost}} with an {{AggregateCatalog}} which it keeps a static reference to. The static member allows  the exported instance of {{DeploymentCatalogService}} to access the aggregate to add new catalogs. The AddXap method takes in a uri which it uses to create a DeploymentCatalog if one does not already exist for that uri. It then subscribes to the DownloadCompleted event and starts the async download. The catalog is then immediately added to aggregate.
# Handling errors during download
As {{DeploymentCatalog}} implements the async event pattern, it does not throw exceptions should any occur duing download. Should an exception occur it will raise the {{DownloadCompleted}} event and set the {{Error}} property on the args class to the Exception. You can see this in the above sample. If an error does occur, you should discard the DeploymentCatalog instance and create a new one. You cannot reset it. It will not cause any issues leaving it in the aggregate catalog however as catalogs that have not downloaded simply return an empty collection of parts.
# Progress updates
{{DeploymentCatalog}} supplies continual notifications during the download. This is useful for supplying a progress bar which displays percentage of download. To receive progress updates, subscribe to the {{DownloadProgressChanged}}. Below you can see that handler for this event.
# Cancelling download
In some scenarios it makes sense to allow users to cancel the download. One case might be if an application is going offline. For these situations {{DeploymentCatalog}} supports a {{CancelAsync}} method. Once you call this method you cannot reuse that catalog. To resume the download you should create a new catalog.
{code:c#}
public class DownloadManagerViewModel, INotifyPropertyChanged {

  public event PropertyChangedEventHandler PropertyChanged;

  public long BytesReceived {get;private set;}

  public int Percentage {get; private set;}

  public void DownloadProcessChanged(object sender, DownloadProgressChangedEventArgs e) {
    BytesReceived = e.BytesReceived;
    Percentage = e.ProgressPercentage;
    RaisePropertyChanged("BytesReceived");
    RaisePropertyChanged("Percentage");
  }

  public void RaisePropertyChanged(string property) {
    var handler = PropertyChanged;
    if(handler!=null) 
      PropertyChanged(new PropertyChangedEventArgs(property));
  }
}
{code:c#}
{code:vb.net}
Public Class DownloadManagerViewModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler
    Private privateBytesReceived As Long
    Public Property BytesReceived() As Long
        Get
            Return privateBytesReceived
        End Get
        Private Set(ByVal value As Long) 
            privateBytesReceived = value
        End Set
    End Property

    Private privatePercentage As Integer
    Public Property Percentage() As Integer
        Get
            Return privatePercentage
        End Get
        Private Set(ByVal value As Integer) 
            privatePercentage = value
        End Set
    End Property

    Public Sub DownloadProcessChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs) 
        BytesReceived = e.BytesReceived
        Percentage = e.ProgressPercentage
        RaisePropertyChanged("BytesReceived")
        RaisePropertyChanged("Percentage")
End Sub

    Public Sub RaisePropertyChanged(ByVal [property](property) As String) 
        Dim handler = PropertyChanged
        If handler IsNot Nothing Then
            RaiseEvent PropertyChanged(New PropertyChangedEventArgs([property](property))) 
        End If
    End Sub
End Class
{code:vb.net}

{{DownloadManagerViewModel}} exposes a TrackDownload method. {{DownloadManagerViewModel}} exposes a {{DownloadProgressChanged}} method which can be called by the application when the event is received. In the event it sets it's BytesReceived and Percentage proeprties to the event args. It then raises the PropertyChanged event in order to notify the UI.
# Using DeploymentCatalog while offline / running out of browser
DeploymentCatalog builds on top of the Silverlight {{WebClient}} class. When an application is offline, that class will automatically leverage the browser cache. This means that previously downloaded XAPs can still be accessed as long as they are in the cache. When the cache is cleared, the XAPs will need to get re-downloaded.
# Caveats to using DeploymentCatalog
* Host most be configured to use it (as is shown).
* Silverlight cached assemblies are not supported including in the main XAP (meaning MEF will not discover them or initiate the download of cached assemblies). There are two work arounds to this. 
	* First you can have the application reference the assemblies that are shared. This will allow XAPs to reference these assemblies with CopyLocal set to False in the references.
	* Second you can have a Shared XAP which contains the assemblies that are shared. As long as the XAP is downloaded before the parts that need it are, then references from those XAPs (CopyLocal=False) will work. This allows you to dynamically introduce new contracts that are not statically referenced.
* Localization is not supported. We grab only the assemblies in the XAP that are defined in the manifest.
* Loose resources/files living outside the assembly cannot be accessed though you can use embedded resources.
* Downloaded catalogs are not copied to the file system when it runs out of browser.

For more on DeploymentCatalog see this post: [uri:http://codebetter.com/blogs/glenn.block/archive/2010/03/07/building-hello-mef-part-iv-deploymentcatalog.aspx](uri_http___codebetter.com_blogs_glenn.block_archive_2010_03_07_building-hello-mef-part-iv-deploymentcatalog.aspx)

