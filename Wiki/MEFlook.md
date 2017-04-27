# Overview
The Meflook sample, as its name suggests, is an email client similar to Outlook. The left panel displays the list of folders available with the number of unread items in parentheses. Selecting a folder will display all mail items in that folder in the middle panel. Any of these items can be read by selecting it. Once selected, it will be displayed in the right panel and will be marked as 'read'.

# How does MEF fit in?
MEF is used to deal with two issues in this sample: laying out the UI and using services to deal with application-wide functionality. The three main panels in Meflook are actually parts that are plugged into the shell and layed out in the appropriate order. Each of these panels also (directly or indirectly) makes use of a SelectionService and an EmailService. The SelectionService provides the app with a means to track the currently-selected folder/email and the EmailService allows for retrieving an the list of available emails and folders, as well as the selected email's contents.

# Important parts
To deal with laying out the UI, the application's shell imports a collection of UserControls through the "MeflookView" contract. In the sample, there are exactly three parts that are exported using the "MeflookView" contract. You've most likely already guessed that these are the that these are the the left panel (AKA the spine), the middle panel (the message list) and the right panel (the message pane). Since all three parts are exported using the same contract, the shell uses the metadata (i.e. the Index property provided via the ShellViewMetadataAttribute) to determine how to order the panels.

The spine also imports a FolderView which allows for displaying the list of email folders as well as selecting a folder. The FolderView, along with the message list and the message pane, all import an email service (through the IEmailService contract type) and a selection service (through the ISelectionService contract type) to keep track of which folders/emails exist and the contents of all emails, as well as the current folder/email selection. MEF easily deals with the singleton pattern that is required for these services.

# Delving deeper into the code
## UI layout
As was stated above, the application shell (MeflookShell) imports all UserControls exported using the "MeflookView" contract. Note that all parts are imported in a delay-loaded fashion (this is done through the use of the ExportCollection<> generic type). The imports also require metadata in the shape of the IShellViewMetadata interface (i.e. the Index metadata is required).

{code:c#}
    public partial class MeflookShell : System.Windows.Window
    {
        [Import("MeflookView")](Import(_MeflookView_))
        private ExportCollection<UserControl, IShellViewMetadata> views = null;

        ...
    }

    [MetadataAttribute](MetadataAttribute)
    public class ShellViewMetadataAttribute : Attribute
    {
        public int Index { get; private set; }

        public ShellViewMetadataAttribute(int index)
        {
            this.Index = index;
        }
    }
{code:c#}
Parts exported using the "MeflookView" contract that are not of type UserControl or do not have the required metadata will not be imported into the 'views' collection. Here's an example of how the LeftSpine exports itself:

{code:c#}
    [Export("MeflookView"), ShellViewMetadata(1)](Export(_MeflookView_),-ShellViewMetadata(1))
    public partial class LeftSpine : System.Windows.Controls.UserControl
    {
        ...
    }
{code:c#}
Notice how the LeftSpine's Index metadata is set to 1. For the MessageList, it is set to 2, and for the MessagePane, it is set to 3. This is used to order the different parts from left to right. The MeflookShell OnLoad event handler takes care of this (at the point when the MeflookShell is added as the application's window, MEF has already finished wiring it up):

{code:c#}
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var view in views.OrderBy(i => i.MetadataView.Index))
            {
                dockPanel.Children.Add(view.GetExportedObject());
            }
        }
{code:c#}
## Services
The FolderView (a sub-part of the LeftSpine), the MessageList and the MessagePane each import an IEmailService and an ISelectionService. Since these are services, there's no point in importing into a collection. The imports are very straightforward and look like this for the FolderView (and are almost identical for the MessageList and MessagePage):

{code:c#}
        [Import(typeof(IEmailService))](Import(typeof(IEmailService)))
        IEmailService emailService = null;

        [Import(typeof(ISelectionService))](Import(typeof(ISelectionService)))
        ISelectionService selectionService = null;
{code:c#}
This is what the exports look like on each respective type:

{code:c#}
    [Export(typeof(IEmailService))](Export(typeof(IEmailService)))
    public class EmailService : IEmailService
    {
        ...
    }

    [Export(typeof(ISelectionService))](Export(typeof(ISelectionService)))
    public class SelectionService : ISelectionService
    {
        ...
    }
{code:c#}
## The entry point
The code that ties it all together resides in the App.xaml.cs:

{code:c#}
public partial class App : Application
{
    private CompositionContainer _container;

    [Import("MainWindow")](Import(_MainWindow_))
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
        catalog.Catalogs.Add(new AttributedAssemblyPartCatalog(typeof(IEmailService).Assembly));
        
        _container = new CompositionContainer(catalog);
        _container.AddPart(this);

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
The App class defines a Compose method which sets up an AggregatingComposablePartCatalog which contains a couple of AttributedAssemblyPartCatalog’s that define the static composition space. This sample doesn’t allow for any dynamic extensibility and primarily uses MEF to decouple and compose its internal state. 

You will notice that the App has a MainWindow property that shadows the MainWindow property on the base Application class. The primary reason for this is so the App can import the MainWindow which is the primary entry point to starting the application. Now since the App has an import it adds itself as a part to the container before composing so it can be setup by the container.

Satisfying the MainWindow (the MeflookShell in this case) import causes a cascade of import satisfaction: the "MeflookView" collection (LeftSpine, MessageList, and MessagePane) in the shell, the FolderView in the LeftSpine and the IEmailService and ISelectionService required by the FolderView, MessageList, and MessagePane.

If all required imports are present (they are in this sample), then the composition succeeds and the App calls Show on the imported MainWindow to get the application off and running.

# Taking this sample one step furter
You might have noticed that the 'Mail', 'Calendar', 'Tasks', 'Contacts' and 'Notes' buttons are hard-coded in the LeftSpine and don't really do anything. This isn't extensible at all, though one might expect it to be. Shouldn't the LeftSpine import a set of parts (instead of one single FolderView) and make these parts available to the user?

These 'views' would be selectable using the 'Mail', 'Calendar', etc. buttons, but again, since these are hard-coded in the LeftSpine, that doesn't allow for much extensibility. Maybe these buttons could be generated on the fly based on the parts' metadata?

Maybe you want to get your hands dirty and try making these changes to the sample?