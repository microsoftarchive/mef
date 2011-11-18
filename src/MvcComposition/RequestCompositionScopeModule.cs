using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace System.ComponentModel.Composition.Web.Mvc
{
    public class RequestCompositionScopeModule : IHttpModule
    {
        static bool _isInitialized;

        public static void Register()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                DynamicModuleUtility.RegisterModule(typeof(RequestCompositionScopeModule));
            }
        }
        
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += DisposeCompositionScope;

            CompositionProvider.PostStartDefaultInitialize();
        }

        static void DisposeCompositionScope(object sender, EventArgs e)
        {
            var scope = CompositionProvider.CurrentInitialisedScope;
            if (scope != null)
                scope.Dispose();
        }
    }
}
