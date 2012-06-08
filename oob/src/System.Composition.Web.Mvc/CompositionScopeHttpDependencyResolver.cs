// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;

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

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}
