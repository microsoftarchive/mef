# Overview
The MefShapes sample is a minimalist video game. Blocks of various shapes and sizes will fall down in the game area on the left. Aligning blocks so that they fill up entire lines in the game area will clear those lines. The objective of the game is to keep the blocks from rising up to the top of the game area by continuously clearing lines.

The set of available shapes is displayed on the left. The File/Open Catalog menu item allows for adding more shapes to the list of available ones. Selecting the MefShapes.AdditionalShapes assembly, for example, will add six more shapes.

# How does MEF fit in?
From the available shapes to the game area itself (AKA the environment), almost every aspect of this sample is a MEF part. The sample's UI shell imports the game itself; the game imports a collection of acceleration strategies (which determines how the blocks accelerate at time progresses, making the game increasingly more difficult), the environment and a collection of available shapes; the shapes import a CellFactory (a means to create the square cells of which the blocks are composed); etc.

The main extensibility point here is the available type of shapes. Selecting the Open Catalog menu item from the File menu opens up a dialog allowing the user to specify an assembly contains IShape parts.

# Important parts
The most interesting aspect of this sample is that IShape parts can be defined in two ways through the use of adapters. First, a type implementing the interface can be exported using the IShape contract type and the required metadata (the type of the shape, a description, and its priority) can be added to the catalog. This is the typical scenario. Second, strings exported with the "MefShapesShapePicture" contract name (again, with the required metadata) will automatically be 'adapted' to the IShape contract. The IMefShapesGame simply imports a collection of exports using the IShape contract and is populated with both sets of shapes. Adapters are used to convert exported parts' original contract to another one.

# Delving deeper into the code
## Adapted parts
The MefShapesGame part imports a collection of delay-loaded IShape parts. These parts are delay-loaded so that the metadata can be queried. Note that the import uses the IShape contract type:

{code:c#}
    public class MefShapesGame:IMefShapesGame
    {
        ...

        [Import](Import)
        [ImportRequiredMetadata("ShapeDescription")](ImportRequiredMetadata(_ShapeDescription_))
        public ObservableCollection<Export<IShape, IShapeMetadata>> SelectionShapes { get; private set; }

        ...
    }
{code:c#}
As expected, the sample contains parts that are exported using the IShape contract. An example of this is the Diagonal part:

{code:c#}
    [Export(typeof(IShape))](Export(typeof(IShape)))
    [Shape(ShapeType.GameShape, "Diagonal shape", 0)](Shape(ShapeType.GameShape,-_Diagonal-shape_,-0))
    [CompositionOptions(CreationPolicy = CreationPolicy.Factory)](CompositionOptions(CreationPolicy-=-CreationPolicy.Factory))
    public class Diagonal : RegularShape
    {
        ...
    }
{code:c#}
However, there are other parts (those initially loaded in the sample, actually) which are specified using strings and another contract:

{code:c#}
    public static class StandardShapePictures
    {
        [Export("MefShapesShapePicture")](Export(_MefShapesShapePicture_))
        [Shape(ShapeType.GameShape, "T shape", 0)](Shape(ShapeType.GameShape,-_T-shape_,-0))
        public const string TShape = "010/111/000";

        [Export("MefShapesShapePicture")](Export(_MefShapesShapePicture_))
        [Shape(ShapeType.GameShape, "L shape", 0)](Shape(ShapeType.GameShape,-_L-shape_,-0))
        public const string LShape = "010/010/011";

        ...
    }
{code:c#}
These parts' contract is converted to an IShape contract with the following adapter:

{code:c#}
    public class PictureShapeAdapter
    {
        [Import](Import)
        public ICompositionService CompositionService { get; set; }

        [Export(CompositionServices.AdapterContractName)](Export(CompositionServices.AdapterContractName))
        [ExportMetadata(CompositionServices.AdapterFromContractMetadataName, "MefShapesShapePicture")](ExportMetadata(CompositionServices.AdapterFromContractMetadataName,-_MefShapesShapePicture_))
        [ExportMetadata(CompositionServices.AdapterToContractMetadataName, typeof(IShape))](ExportMetadata(CompositionServices.AdapterToContractMetadataName,-typeof(IShape)))
        public Export Adapt(Export export)
        {
            //  Note that this creates an Export with the IsSingleton property set to true,
            //  even though it will actually behave as a factory (ie it will create a new instance 
            //  each time GetExportedObject is called).  The factory behavior is what we want.
            return Export.Create(() =>
            {
                string picture = (string)export.GetExportedObject();
                var ret = new PictureShape(picture);
                CompositionService.SatisfyImports(CompositionServices.CreateAttributedPart(ret));

                return ret;
            }, export.Metadata);
        }
    }
{code:c#}
There's a lot of stuff happening up there, so here's the breakdown:
# The PictureShapeAdapter.Adapt(Export) method is exported with the _CompositionServices.AdapterContractName_ contract. This informs MEF that this method will be used to adapt from one contract to another.
# There are two pieces of metadata required for an adapter: _CompositionServices.AdapterFromContractMetadataName_ and _CompositionServices.AdapterToContractMetadataName_. These, as the names suggest, specify the original contract and the target contract. These can be strings or types.
# The Adapt method takes in an Export of the part as it is originally described and returns another of the part as it is described in its adapted form. This includes both the delegate that converts the value and the new metadata.
# The call to CompositionService.SatisfyImports is required to ensure that the adapted part has its imports properly wired up.

## The entry point
Much like the other samples, the host application (from within app.cs) imports the MainWindow part (defined in MainWindow.xaml.cs) which causes the entire application to compose:

{code:c#}
public partial class App : Application
{
    private CompositionContainer _container;

    [STAThreadAttribute()](STAThreadAttribute())
    public static void Main()
    {
        new App().Run();
    }

    [Import(typeof(MainWindow))](Import(typeof(MainWindow)))
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
        catalog.Catalogs.Add(new AttributedAssemblyPartCatalog(typeof(IMefShapesGame).Assembly));
        catalog.Catalogs.Add(new AttributedAssemblyPartCatalog(typeof(DefaultDimensions).Assembly));

        _container = new CompositionContainer(catalog);

        _container.AddPart(this);
        _container.AddExportedObject<ICompositionService>(_container);
        _container.AddExportedObject<AggregatingComposablePartCatalog>(catalog);

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

The App class defines a Compose method which sets up an AggregatingComposablePartCatalog which contains a couple of AttributedAssemblyPartCatalog’s that define the static composition space. However, you will notice that the catalog is added as an export to the container which allows someone to import it. In this sample the MainWindow imports it and allows a user to actually add new assemblies to the catalog at runtime to support dynamically adding new shapes.

You will notice that the App has a MainWindow property that shadows the MainWindow property on the base Application class. The primary reason for this is so the App can import the MainWindow which is the primary entry point to starting the application. Now since the App has an import it adds itself as a part to the container before composing so it can be setup by the container.

Assuming the composition succeeded then the App calls Show on the imported MainWindow to get the application off and running.