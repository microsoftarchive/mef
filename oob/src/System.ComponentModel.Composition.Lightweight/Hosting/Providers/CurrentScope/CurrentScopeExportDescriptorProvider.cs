// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.CurrentScope
{
    class CurrentScopeExportDescriptorProvider : ExportDescriptorProvider
    {
        public override ExportDescriptorPromise[] GetExportDescriptors(Contract contract, DependencyAccessor definitionAccessor)
        {
            if (contract.ContractType != typeof(IExportProvider) || contract.Discriminator != null)
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                typeof(IExportProvider).Name,
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => c, NoMetadata)) };
        }
    }
}
