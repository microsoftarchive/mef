// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;


namespace System.ComponentModel.Composition
{
    static class ScopingHelpers
    {
        public static CompositionScopeDefinition AsScope(this ComposablePartCatalog catalog, params CompositionScopeDefinition[] children)
        {
            return new CompositionScopeDefinition(catalog, children);
        }

        public static CompositionScopeDefinition AsScopeWithPublicSurface<T>(this ComposablePartCatalog catalog, params CompositionScopeDefinition[] children)
        {
            IEnumerable<ExportDefinition> definitions = catalog.Parts.SelectMany( (p) => p.ExportDefinitions.Where( (e) => e.ContractName == AttributedModelServices.GetContractName(typeof(T)) ) );
            return new CompositionScopeDefinition(catalog, children, definitions);
        }
    }
}
