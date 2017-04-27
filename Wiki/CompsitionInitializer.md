# Hosting MEF in Silverlight
In the desktop today in order to use MEF you are required to manually configure the CompositionContainer (and catalogs) in order to allow the application to discover parts. That container often needs to be exposed (preferably not directly but through a wrapper) and passed around to other parts of the application that may need it for dynamically composing new parts. 

In Silverlight we've introduced a new api called {{System.ComponentModel.Composition.CompositionInitializer}} which resides in System.ComponentModel.CompositionInitialization assembly. CompositionInitializer allows parts to get composed by MEF without having to do any manual bootstrapping as it will automatically configure MEF on demand. With CompositionInitializer any class that has been newed up can have imports and MEF will satisfy them. This means you can use it anywhere within your Silverlight application. A common place to use it is within your App class in order to import the MainView, or the MainViewViewModel if you are applying an MVVM pattern as in the sample below.

{code:c#}
using System.ComponentModel.Composition;

public class App : Application {
  public App() {
    this.Startup += this.Application_Startup;
    this.Exit += this.Application_Exit;
    this.UnhandledException += this.Application_UnhandledException;

    InitializeComponent();
  }

  private void Application_Startup(object sender, StartupEventArgs e)
  {
    CompositionInitializer.SatisfyImports(this)
    var mainPage = new MainPage();
    mainPage.DataContext = ViewModel;
    RootVisual = mainPage;
  }

  [Import](Import)
  public MainViewModel ViewModel {get;set;}
}

[Export](Export)
public class MainViewModel, INotifyPropertyChanged {
  [Import](Import)
  public ILogger Loger {get;set;}
}

[Export(typeof(ILogger))](Export(typeof(ILogger)))
public class Logger : ILogger {
}
{code:c#}
{code:vb.net}
Imports System.ComponentModel.Composition

Partial Public Class App
    Inherits Application

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        CompositionInitializer.SatisfyImports(Me) 
        Dim mainPage_Renamed = New MainPage()
        mainPage_Renamed.DataContext = ViewModel
        RootVisual = mainPage_Renamed
    End Sub

    <Import()>
    Public Property ViewModel() As MainViewModel

End Class

<Export()>
Public Class MainViewModel
    Implements INotifyPropertyChanged
    <Import()>
    Public Property Loger() As ILogger
End Class

<Export(GetType(ILogger))> 
Public Class Logger
    Implements ILogger
End Class
{code:vb.net}


The App imports a MainViewModel. Then in the Application_Startup method it calls CompositionInitializer passing _itself_ in order to have it's imports satisfied. This causes MainViewModel to be discovered by MEF and injected with a Logger instance. The application then creates a MainView instance and sets the DataContext to the imported ViewModel. 

_What's really happening?_

The first time SatisfyImports is called, CompositionInitializer is creating a new shared container behind the scenes which it will use for future calls to SatisfyImports. The catalog for that container contains all parts discovered within the assemblies in the application's main XAP. This means that in addition to the current executing assembly, all referenced assemblies (and their references) also have their parts discovered.

## Exports not allowed on the instance passed to SatisfyImports.
Notice that the Application class does not have an export on it. {{SatisfyImports}} is designed to be used ONLY for parts that cannot be discovered by the catalog, meaning they do not have exports. It will throw an exception if you pass it a class that has an {{ExportAttribute}} on it

## Using CompositionInitializer from within XAML created elements.
CompositionInitializer is designed to be called mulitple times with it configuring itself on the first call. This makes it ideal to use not only within the root Application class but also within elements created in XAML. For example see the xaml below.

{code:xml}
<UserControl
 x:Class="OrderManagement.OrderView"
 xmlns:c="clr-namespace:MyApp.Controls">
  <StackPanel>
    <OrderHeader/>
    <OrderDetails/>
  </StackPanel>
<UserControl>
{code:xml}

OrderHeader and OrderDetails are nested controls within OrderView which are created in XAML. Both however have their own respective view models which they import. For example below is the OrderHeader. Notice it does not have an export.

{code:c#}
public class OrderHeader : UserControl {
  [Import](Import)
  public OrderHeaderViewModel ViewModel {get;set;}

  public OrderHeader() {
    CompositionInitializer.SatisfyImports(this);
    this.DataContext = ViewModel;
  }
}
{code:c#}
{code:vb.net}
Public Class OrderHeader
    Inherits UserControl
    <Import()>
    Public Property ViewModel() As OrderHeaderViewModel

    Public Sub New()
        CompositionInitializer.SatisfyImports(Me) 
        Me.DataContext = ViewModel
    End Sub

End Class
{code:vb.net}

In this case OrderHeader is directly importing it's ViewModel versus having it externally wired as in the MainView case. In this case we did this in order to allow it simply to be dropped within XAML without pushing any knowledge of it's wiring up to the containing control. This type of pattern makes sense in particular for third-party controls using MEF as it simplifies the usage of the control. It's a matter of preference though.

## Caveats about using CompositionInitializer.SatisfyImports
# By default only assemblies in the current XAP are discovered. You can override this behavior, to learn about this see the overriding host config [topic](OverridingHostConfig).
# All parts created with CompositionInitializer are held around by MEF until the application shuts down. Thus it is not ideal to use for composing transient multiple-instance parts. For these cases a better choice is to use [ExportFactory](PartCreator)
# As mentioned above Exports are not supported.