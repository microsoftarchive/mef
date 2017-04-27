# Overview
This MEF sample app lets you explore your hard disk, much in the same way windows explorer does, but with a few differences thrown in the mix. As expected, the leftmost part of the UI (refered to as the FolderView) allows you to navigate folders. The right part of the UI (with the tabs), will display the contents of the folder. Notice that there are two tabs, the 'Contents Pane' and the 'Size Pane'. The Contents Pane will allow you to navigate the contents of the folder and preview certain types of files (pictures and text documents are supported, more on that later). The Size Pane displays a pie chart of the largest items in the folder.

At the top, you'll find the typical address bar; at the bottom, the status bar and the 'Favorites' bar. You can add favorites by right-clicking on an item in the Content pane and selecting 'Add To Favorites' from the 'Custom' sub-menu.

Also, each part can be disabled/enabled from the 'View' menu.

# How does MEF fit in?
MEF is used a great deal in this sample. All main panels in the UI are seperate parts including the address bar, the status bar, the favorites bar, the folder view, the content view (on the Content Pane tab), the preview pane (also on the Content Pane tab) as well as the Size Pane tab. Note that the different parts of the UI are not layed out in a straight-forward manner (i.e. by a single Panel, like a StackPanel). This is dealt with by the application shell part. Also, some UI parts depend on other sub-parts (for example, the preview pane depends on different parts selected file is an image or if it is a a text file). 

These are the main extensibility points for this application: new UI parts will add more features, and new sub-parts will add more functionality to existing UI parts.

There are also a few services which allow for changing state shared amongst all parts such as a navigation service that tracks the currently-select folder/file and an icon reader service that manages icons in the favorites bar.

# Important parts
The Extensible File Explorer's UI revolves around a set of parts exported with the "Microsoft.Samples.XFileExplorer.FileExplorerViewContract" contract name (namely, the AddressView, StatusView, FavoritesPane, FolderView, ContentView, PreviewView and SizeView). Based on the associated metadata, the application shell (which imports them) has logic to determine whether one of these parts should be dock at the top, the bottom, the left or the right. This is done by looking at the "Docking" metadata. Docking on the right is special-cased here. All parts docking on the right provide a tab index ("DockId" metadata). This will determine which tab the part will land on.

Some of these 'FileExplorerViews' depend on other parts for functionality. This is the case for the StatusView, the FavoritesView and the PreviewView. For example, the PreviewView imports all parts exported using the "Microsoft.Samples.XFileExplorer.PreviewServiceContract" contract name. These parts are expected to provide the functionality necessary to deal with different types of previews. The PreviewViewImg and PreviewViewTxt (both parts exported with that contract name) afford previewing images (for the PreviewViewImg) and text (PreviewViewTxt). Adding more parts that satisfy this contract will provide 'preview' functionality for more types (for example, one could imagine an Excel file preview).

The StatusView imports all parts satisfying the "Microsoft.Samples.XFileExplorer.StatusServiceContract" contract (i.e. StatusViewFilesSize and StatusViewItemCount) which help display information in the status bar. Similarly, the FavoritePane imports parts exported using the IFavoriteItem contract type (i.e. FavoriteFileItem and FavoriteDirectoryItem). These parts describe how to retrieve and display a favorite.

There are two services required by the sample: INavigationService and IIconReaderService. As the name implies, the INavigationService affords browsing through/selecting folders and files. All parts that import the INavigationService will have access to the currently-selected folder and file and will have the ability to change them. The IIconReaderService is a little less intuitive at first. It is required by both the ContentView and items on the FavoritePane parts. These parts need to draw the icons of the files they're displaying, so they query the IIconReaderService to get the right image.

# Delving deeper into the code
## UI parts
The application shell imports parts exported using the "Microsoft.Samples.XFileExplorer.FileExplorerViewContract" contract name. The parts are imported in a delay-loaded fashion to allow for querying their metadata. The metdata will be used to determine how to lay out the part in the UI:

{code:c#}
        [Import("Microsoft.Samples.XFileExplorer.FileExplorerViewContract")](Import(_Microsoft.Samples.XFileExplorer.FileExplorerViewContract_))
        private ExportCollection<UserControl, IFileExplorerViewMetadata> _views = null;
{code:c#}

The parts satisfying this contract must also export the metadata described by the IFileExplorerViewMetadata interface to be imported. However, some of the interface's properties are decorated with the DefaultValue attribute. This specifies that these properties are not required metadata on the export and will take on the default value if not exported:

{code:c#}
    public interface IFileExplorerViewMetadata
    {
        string Name { get; }

        Dock Docking { get; }

        [DefaultValue(0)](DefaultValue(0))
        int DockId { get; }

        [DefaultValue(false)](DefaultValue(false))
        bool Hidden { get; }
    }
{code:c#}
The part specifies how it should be displayed on the UI through the Docking property (Top, Bottom, Left or Right). As stated above, a value of Dock.Right will place the part in one of the tabs of the tab control on the right. The index of the tab where the part will be placed is specified by the _DockId_ property (which defaults to 0, the first tab). This is the case for the PreviewView part, for example:

{code:c#}
    [Export("Microsoft.Samples.XFileExplorer.FileExplorerViewContract")](Export(_Microsoft.Samples.XFileExplorer.FileExplorerViewContract_))
    [ExportMetadata("Name", "Preview Pane")](ExportMetadata(_Name_,-_Preview-Pane_))
    [ExportMetadata("Docking", Dock.Right)](ExportMetadata(_Docking_,-Dock.Right))
    [ExportMetadata("DockId", 0)](ExportMetadata(_DockId_,-0))
    public partial class PreviewView : UserControl
    {
        ...
    }
{code:c#}

## Parts providing functionality
The PreviewView part, for example, depends on a collection of other parts (imported using the _PreviewControl_ as the contract type) which will each provide a means for displaying a preview of specific file types and the contract type enforces a preview part must have a _UpdatePreview()_ method:

{code:c#}
    public partial class PreviewView : UserControl
    {
        ...
        [Import(typeof(PreviewControl))](Import(typeof(PreviewControl)))
        public ExportCollection<PreviewControl, IPreviewMetadata> Viewers { set; get; }
        ...
    }

    [ContractType("Microsoft.Samples.XFileExplorer.PreviewServiceContract", MetadataViewType = typeof(IPreviewMetadata))](ContractType(_Microsoft.Samples.XFileExplorer.PreviewServiceContract_,-MetadataViewType-=-typeof(IPreviewMetadata)))
    public abstract class PreviewControl : UserControl
    {
        public abstract void UpdatePreview();
    }

    public interface IPreviewMetadata
    {
        IList<string> Format { get; }
    }
{code:c#}

The metadata is expected to list out all the formats that are supported by the 'PreviewService'. To do so, the _IsMultiple_ property on each _ExportMetadata_ attribute needs to be set to _true_:

{code:c#}
    [Export(typeof(PreviewControl))](Export(typeof(PreviewControl)))
    [ExportMetadata("Format", "BMP", IsMultiple = true)](ExportMetadata(_Format_,-_BMP_,-IsMultiple-=-true))
    [ExportMetadata("Format", "JPG", IsMultiple = true)](ExportMetadata(_Format_,-_JPG_,-IsMultiple-=-true))
    [ExportMetadata("Format", "JPGE", IsMultiple = true)](ExportMetadata(_Format_,-_JPGE_,-IsMultiple-=-true))
    [ExportMetadata("Format", "PNG", IsMultiple = true)](ExportMetadata(_Format_,-_PNG_,-IsMultiple-=-true))
    [ExportMetadata("Format", "GIF", IsMultiple = true)](ExportMetadata(_Format_,-_GIF_,-IsMultiple-=-true))
    [ExportMetadata("Format", "ICO", IsMultiple = true)](ExportMetadata(_Format_,-_ICO_,-IsMultiple-=-true))
    [ExportMetadata("Format", "TIFF", IsMultiple = true)](ExportMetadata(_Format_,-_TIFF_,-IsMultiple-=-true))
    [ExportMetadata("Format", "TIF", IsMultiple = true)](ExportMetadata(_Format_,-_TIF_,-IsMultiple-=-true))
    [ExportMetadata("Format", "WDP", IsMultiple = true)](ExportMetadata(_Format_,-_WDP_,-IsMultiple-=-true))
    public partial class PreviewViewImg : PreviewControl
    {
        ...
    }
{code:c#}
According to its contract, this part should be able to preview image files for which the extension is BMP, JPG, JPEG, PNG, GIF, ICO, TIFF, TIF and WDP. Calling the part's _UpdatePreview()_ method is all that's needed to display the preview.

## The entry point
The App class is the host for the primary CompositionContainer for the application. It is in charge of setting up the catalog and container to get the root objects such as MainWindow composed.

{code:c#}
public partial class App : Application
{
    private CompositionContainer _container;

    [Import("Microsoft.Samples.XFileExplorer.MainWindowContract")](Import(_Microsoft.Samples.XFileExplorer.MainWindowContract_))
    public new Window MainWindow
    {
        get { return base.MainWindow; }
        set { base.MainWindow = value; }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Compose())
        {
            MainWindow.Show();
        }
        else
        {
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        if (_container != null)
        {
            _container.Dispose();
        }
    }

    private bool Compose()
    {
        var catalog = new AggregatingComposablePartCatalog();
        catalog.Catalogs.Add(new AttributedAssemblyPartCatalog(Assembly.GetExecutingAssembly()));
        catalog.Catalogs.Add(new DirectoryPartCatalog("AddIns", true));

        _container = new CompositionContainer(catalog);
        _container.AddPart(this);
        _container.AddExportedObject<ICompositionService>(_container);

        try
        {
            _container.Compose();
        }
        catch (CompositionException compositionException)
        {
            MessageBox.Show(compositionException.ToString());
            return false;
        }
        return true;
    }
}
{code:c#}

The App class defines a Compose method which sets up an AggregatingComposablePartCatalog which contains a DirectoryPartCatalog and an AttributedAssemblyPartCatalog. The DirectoryPartCatalog is a key extensibility point here. It basically watches the “AddIns” directory for changes and will update the catalog when assemblies are added to the directory. Therefore anyone can add their own UI part, for example, by placing an assembly containing a type exported using the "Microsoft.Samples.XFileExplorer.FileExplorerViewContract" contract in the “AddIns” directory.

You will notice that the App has a MainWindow property that shadows the MainWindow property on the base Application class. The primary reason for this is so the App can import the MainWindow which is the primary entry point to starting the application. Now since the App has an import it adds itself as a part to the container before composing so it can be setup by the container.

Assuming the composition succeeded then the App calls Show on the imported MainWindow to get the application off and running.