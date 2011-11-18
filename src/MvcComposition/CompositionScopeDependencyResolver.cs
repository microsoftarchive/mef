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
            return CompositionProvider.Current.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(serviceType));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return CompositionProvider.Current.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }
    }
}