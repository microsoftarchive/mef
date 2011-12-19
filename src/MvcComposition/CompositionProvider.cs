using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace System.ComponentModel.Composition.Web.Mvc
{
    public class CompositionProvider
    {
        public const string ApplicationShared = "CompositionProvider.ApplicationShared";

        static CompositionContainer _applicationScopeContainer;
        static ComposablePartCatalog _perRequestCatalog;
        static IList<Assembly> _partAssemblies = new List<Assembly>();

        static public void SetCatalog(ComposablePartCatalog catalog)
        {
            if (catalog == null) throw new ArgumentNullException("catalog");
            if (_perRequestCatalog != null) throw new InvalidOperationException("Already initialized.");

            var globals = catalog.Filter(cpd => cpd.ContainsPartMetadata(ApplicationShared, true));
            _applicationScopeContainer = new CompositionContainer(globals, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            _perRequestCatalog = globals.Complement;

            DependencyResolver.SetResolver(new CompositionScopeDependencyResolver());

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().SingleOrDefault());
            FilterProviders.Providers.Add(new CompositionScopeFilterAttributeFilterProvider());

            ModelBinderProviders.BinderProviders.Add(new CompositionScopeModelBinderProvider());
        }

        internal static CompositionContainer Current
        {
            get
            {
                return CurrentInitialisedScope ?? (CurrentInitialisedScope = new CompositionContainer(_perRequestCatalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe, _applicationScopeContainer));
            }
        }

        internal static CompositionContainer CurrentInitialisedScope
        {
            get { return (CompositionContainer)HttpContext.Current.Items[typeof(CompositionProvider)]; }
            private set { HttpContext.Current.Items[typeof(CompositionProvider)] = value; }
        }


        public static void AddPartsAssemblies(params Assembly[] partsAssemblies)
        {
            AddPartsAssemblies((IEnumerable<Assembly>)partsAssemblies);
        }

        public static void AddPartsAssemblies(IEnumerable<Assembly> partsAssemblies)
        {
            foreach (var assembly in partsAssemblies)
                AddPartsAssembly(assembly);
        }

        public static void AddPartsAssembly(Assembly partsAssemblies)
        {
            if (partsAssemblies == null)
                throw new ArgumentNullException("partsAssemblies");

            _partAssemblies.Add(partsAssemblies);
        }

        internal static void PostStartDefaultInitialize()
        {
            if (!IsInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Performing default composition initialization.");
                SetCatalog(new MvcApplicationCatalog(
                    _partAssemblies.Union(new[] { MvcApplicationCatalog.GuessGlobalApplicationAssembly() }).ToArray()));
            }
        }

        static bool IsInitialized
        {
            get { return _applicationScopeContainer != null; }
        }
    }
}