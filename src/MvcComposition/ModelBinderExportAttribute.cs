using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.ComponentModel.Composition.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class ModelBinderExportAttribute : ExportAttribute
    {
        public ModelBinderExportAttribute(Type modelType)
            : base(CompositionScopeModelBinderProvider.GetModelBinderContractName(modelType), typeof(IModelBinder))
        {
        }
    }
}
