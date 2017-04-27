# Overview 
The PictureViewer sample is a simple Silverlight client to browse through pictures downloaded from the web. Currently, it only downloads three sample albums but could easily be extended to allow more albums or more complex albums.

# How does MEF fit in? 
MEF is used to demonstrate 3 principles in this sample. First, several UI elements use [import](import) to notify the system they are interested in certain components, and MEF can satisfy those imports without having to resort to hard-coding. Second, several components can have multiple variations that could be replaced at runtime. You could have multiple authentication modules to allow the viewer to log into different systems, or you could replace the types of views that are enabled. Third, light-up scenarios are easily enabled. This is demonstrated by the albums package being loaded on-demand, only after the user logs in.

# Important parts 
First, during Application_Startup, the app prepares the main window, sets it as the root visual, and prepares it for compostion. The initial composition adds all of the assemblies in the current xap to the container using AddPackage(Package.Current). It also adds a second PackageCatalog, wraps it in a part, which is then added to the container. This part is imported by the PhotoAlbumsProvider to add packages after the login without having to connect back to the container. After the Loaded event is fired on the main window, it adds several parts to the container for composition. This way, each UI element can have its imports satisfied when the requisite parts come available.

Second, (look at how each part handles its imports)
* Browser.xaml.cs – Imports PhotoService to connect the Albums to the TreeView (shown on the right hand side)
* Controls.xaml.cs – Imports PhotoService toallow control over which album and/or which picture are being viewed at any given point in time.
* LoginDialog.xaml.cs – Imports AuthenitcationDataProvider(s) (currently bypassed) and PhotoService.
	* By importing multiple AuthenticationDataProviders, the application could connect to multiple services to display pictures.
	* By importing PhotoService, the Login process can add albums directly to the PhotoService so they can be displayed immediately after logging in.
* Viewer.xaml.cs – Imports Views which allow the user to display albums and pictures in one of three modes. These views are discovered in the appropriate assemblies and automatically wired up.
	* Picture View – Show a single picture at a time from a given Album.
	* Album View - Shows all pictures in the selected album
	* Shows all pictures in all selected albums. (This is found in the PictureViewer.Extensions assembly)
	* Each view also [Imports](Imports) Photo Service to allow them to see what the selected Album and Pictures are.
Third, the albums are loaded dynamically. This covers the light-up scenario. After the authentication is completed, the PhotoAlbumProviderStub is called to notify that authentication is completed. It then calls Package.DownloadPackageAsync to download the pre-created package with the albums. This is a complete xap with the redundant assemblies filtered out. After it is added to the container, it will cause recomposition which will allow the albums to show up. Immediately upon calling Catalog.AddPackage on the PackageCatalog, the albums will show up with no additional work.

# Delving deeper into the code 
## Light-up 
Note how it only takes a few lines of code to download additional albums and “light-up” the album list!
{code:c#}
            // Download the package here
            Package.DownloadPackageAsync(new Uri("AlbumPackages.xap", UriKind.Relative), (e, p) =>
            {

                if (!e.Cancelled && e.Error == null)
                {
                    // Cause recomposition
                    CatalogPart.Catalog.AddPackage(p);
                }
            });
{code:c#}
{code:vb.net}
            'Download the package here
            Package.DownloadPackageAsync(New Uri("AlbumPackages.xap", UriKind.Relative), Sub(e, p)
                		If (Not e.Cancelled) AndAlso e.Error Is Nothing Then
                    		'Cause recomposition
                   			 CatalogPart.Catalog.AddPackage(p)
                		End If
		End Sub)
{code:vb.net}


# Photo Service
The Photo Service is the heart of the application. It is imported into the various components that need its services, and a single instance is shared throughout the whole application.
{code:c#}
[Import](Import)
public IPhotoService PhotoService
{code:c#}
{code:vb.net}
<Import>
Public Property PhotoService() As IPhotoService
{code:vb.net}

# Wiring up the individual components 
Note: This will change in a future update to MEF on Silverlight/WPF.

In order for the individual parts to get access to their imports, they are added to the container and as a batch, and then the batch is composed. This is done in the Loaded event of MainWindow to ensure they are created and added to the tree.
{code:c#}
// Field access to the parts is not the recommended way to handle
// recomposition.
// We plan to address this pattern with future updates
CompositionBatch batch = new CompositionBatch();
batch.AddPart(mw.viewer);
batch.AddPart(mw.browser);
batch.AddPart(mw.controlButtons);
batch.AddPart(mw.loginDialog);

_container.Compose(batch); 
{code:c#}
{code:vb.net}
' Field access to the parts is not the recommended way to handle
' recomposition.
' We plan to address this pattern with future updates
Dim batch As New CompositionBatch()
batch.AddPart(mw.viewer)
batch.AddPart(mw.browser)
batch.AddPart(mw.controlButtons)
batch.AddPart(mw.loginDialog)

_container.Compose(batch)
{code:vb.net}

# Taking this sample one step further 
There are several things that can be done to enhance this application. For example, Authentication can be re-enabled, and additional IAuthenticatedDataProviders could be created to allow access to different photo sharing sites.

Wiring up of the components could be done in a different manner. (As stated previously, this will be addressed in a future update to MEF on Silverlight)

The controls could be refined to only display the appropriate buttons for any given view. If the View is CollectedAlbumView, no controls are necessary, and if the AlbumView is active, only Next & Previous Album buttons are necessary.