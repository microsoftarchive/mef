# Creating a self-host dependency resolver for ASP.NET Web API using Microsoft.Composition

ASP.NET Web API’s {{IDependencyResolver}} interface allows {{IHttpController}}s to accept dependencies using dependency injection. By plugging in MEF via this interface, controllers can import their dependencies using the familiar techniques of constructor and property injection.

This page describes the creation and usage of an {{IDependencyResolver}} based on the _Microsoft.Composition_ NuGet package. Source code from the example is in the _/oob/src/System.Composition.Web.Http_ folder in the source code repository on this site.

## IDependencyScope

Web API requests exports (which it calls 'services') from an {{IDependencyScope}}. The dependency scope can be used to control sharing on a per-API-call basis, and allows parts to be disposed when no longer required by Web API.

We call this a “standalone” dependency resolver because it is intended for use in self-hosting scenarios; when using Web API with ASP.NET MVC it may be preferable to bridge the Web API dependency resolver across to the MVC one.

{{    public class StandaloneDependencyScope : IDependencyScope
    {
}}

MEF for web and Windows Store apps uses composition contexts to represent scopes. To allow the scope to be disposed when no longer required by Web API, an {{Export<CompositionContext>}} is used:

{{        readonly Export<CompositionContext> _compositionScope;

        public StandaloneDependencyScope(Export<CompositionContext> compositionScope)
        {
            if (compositionScope == null) throw new ArgumentNullException("compositionScope");
            _compositionScope = compositionScope;
        }

        protected CompositionContext CompositionScope { get { return _compositionScope.Value; } }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
                _compositionScope.Dispose();
        }
}}

Web API uses two methods – {{GetService()}} and {{GetServices()}} – to request exports from the scope. Rather than throwing on failure, Web API requires that we return {{null}} if a service is unavailable:

{{        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            object result;
            CompositionScope.TryGetExport(serviceType, null, out result);
            return result;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            return CompositionScope.GetExports(serviceType, null);
        }
    }
}}

## IDependencyResolver

{{IDependencyResolver}} serves two purposes:

* It is used by WebAPI to create an {{IDependencyScope}} for each incoming API call
* It is itself an {{IDependencyScope}} used by Web API to access services that are required for the duration of the application’s lifetime

First, to provide the required dependency scope behavior, the dependency resolver derives from the dependency scope type:

{{    public class StandaloneDependencyResolver : StandaloneDependencyScope, IDependencyResolver
    {
}}

The dependency resolver is initialized using a {{CompositionHost}} object; this is wrapped in an {{Export<CompositionContext>}} so that it can satisfy the base class constructor requirements:

{{        readonly ExportFactory<CompositionContext> _requestScopeFactory;

        public StandaloneDependencyResolver(CompositionHost rootCompositionScope)
            : base(new Export<CompositionContext>(rootCompositionScope, rootCompositionScope.Dispose))
        {
            if (rootCompositionScope == null) throw new ArgumentNullException();
            var factoryContract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null, new Dictionary<string, object> {
                { "SharingBoundaryNames", new[]() { "HttpRequest" } }
            });

            _requestScopeFactory = (ExportFactory<CompositionContext>)rootCompositionScope.GetExport(factoryContract);
        }

        public IDependencyScope BeginScope()
        {
            return new StandaloneDependencyScope(_requestScopeFactory.CreateExport());
        }
    }
}}

The dependency resolver creates new dependency scopes whenever the {{BeginScope()}} method is called. By setting up the {{_requestScopeFactory}} as a sharing boundary, parts can be shared within the scope of an API call. An example of a shared part is shown below:

{{        [Shared("HttpRequest")](Shared(_HttpRequest_))
        public class EFRepository : IRepository { }
}}

Parts like the {{EFRepository}} class are placed in a _/Parts_ folder and namespace by convention – see below.

It is recommended that string constants (rather than literals like “HttpRequest” as the example shows) are used for sharing boundary names.

## Initialization

The dependency resolver needs to be registered with Web API when the application starts up. Here, we create a convention for parts, so that any classes in the _/Parts_ namespace within the application assembly will be used as parts for composition, in addition to API controllers:

{{    private static void InitializeDependencyResolver(Assembly[]() appAssemblies)
    {
        var conventions = new ConventionBuilder();

        conventions.ForTypesDerivedFrom<IHttpController>()
            .Export();

        conventions.ForTypesMatching(t => t.Namespace != null &&
                  (t.Namespace.EndsWith(".Parts") || t.Namespace.Contains(".Parts.")))
            .Export()
            .ExportInterfaces();

        var container = new ContainerConfiguration()
            .WithAssemblies(appAssemblies, conventions)
            .CreateContainer();

        System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new StandaloneDependencyResolver(container);
    }
}}

## Usage

Once the dependency resolver has been initialized, API controllers can accept constructor dependencies, for example the {{IRepository}} interface provided by the {{EFRepository}}:

{{    // Constructor on a controller class
    public ContactsController(IRepository repository) { }
}}