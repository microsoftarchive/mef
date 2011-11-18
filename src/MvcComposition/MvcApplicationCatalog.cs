using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition.Registration;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Web.Mvc
{
    public class MvcApplicationCatalog : ComposablePartCatalog
    {
        ComposablePartCatalog _inner;

        MvcApplicationCatalog(IEnumerable<Assembly> assemblies, ReflectionContext reflectionContext)
        {
            _inner = new AggregateCatalog(assemblies.Select(a => new AssemblyCatalog(a, reflectionContext)));
        }

        public MvcApplicationCatalog(IEnumerable<Assembly> assemblies)
            : this(assemblies, DefineConventions())
        {
        }

        public MvcApplicationCatalog()
            : this(new[] { GuessGlobalApplicationAssembly() })
        {
        }

        internal static Assembly GuessGlobalApplicationAssembly()
        {
            // Purely based on the quirks of ASP.NET compilation, need to find
            // a more reliable way of doing this.
            return HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
        }

        private static ReflectionContext DefineConventions()
        {
            var rb = new RegistrationBuilder();

            rb.ForTypesDerivedFrom<IController>()
                .Export();

            rb.ForTypesMatching(IsAPart)
                .Export()
                .ExportInterfaces(t => t != typeof(IDisposable));

            rb.ForTypesMatching(t => IsAPart(t) && t.GetCustomAttributes(typeof(ApplicationSharedAttribute), true).Any())
                .AddMetadata(CompositionProvider.ApplicationShared, true);

            return rb;
        }

        private static bool IsAPart(Type t)
        {
            return !t.Name.EndsWith("Attribute") &&
                                    t.Namespace != null &&
                                    t.IsInNamespace("Parts");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _inner.Dispose();

            base.Dispose(disposing);
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _inner.Parts;
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return _inner.GetExports(definition);
        }
    }
}
