using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Services;

namespace System.Composition.Web.Mvc
{
    class CompositionScopeHttpDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            object export;
            if (!CompositionProvider.Current.TryGetExport(serviceType, null, out export))
                return null;

            return export;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return CompositionProvider.Current.GetExports(serviceType);
        }
    }
}
