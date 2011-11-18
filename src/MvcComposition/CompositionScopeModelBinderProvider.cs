using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.ComponentModel.Composition.Web.Mvc
{
    class CompositionScopeModelBinderProvider : IModelBinderProvider
    {
        const string ModelBinderContractNameSuffix = "++ModelBinder";

        public static string GetModelBinderContractName(Type modelType)
        {
            return AttributedModelServices.GetContractName(modelType) + ModelBinderContractNameSuffix;
        }

        public IModelBinder GetBinder(Type modelType)
        {
            return CompositionProvider.Current.GetExportedValueOrDefault<IModelBinder>(GetModelBinderContractName(modelType));
        }
    }
}
