// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace System.ComponentModel.Composition.Web.Mvc
{
    class CompositionScopeDependencyResolver : IDependencyResolver
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