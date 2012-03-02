// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.ComponentModel.Composition.Web.Mvc
{
    class CompositionScopeFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        public CompositionScopeFilterAttributeFilterProvider()
            : base(cacheAttributeInstances: false) { }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToArray();
            ComposeAttributes(attributes);
            return attributes;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToArray();
            ComposeAttributes(attributes);
            return attributes;
        }

        void ComposeAttributes(FilterAttribute[] attributes)
        {
            foreach (var attribute in attributes)
                CompositionProvider.Current.SatisfyImports(attribute);
        }
    }
}
