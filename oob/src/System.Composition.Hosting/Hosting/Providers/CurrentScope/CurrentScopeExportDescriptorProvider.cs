// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;

namespace System.Composition.Hosting.Providers.CurrentScope
{
    class CurrentScopeExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly CompositionContract CurrentScopeContract = new CompositionContract(typeof(CompositionContext));

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (!contract.Equals(CurrentScopeContract))
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                typeof(CompositionContext).Name,
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => c, NoMetadata)) };
        }
    }
}
