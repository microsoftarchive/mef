// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    class DiscoveredInstanceExport : DiscoveredExport
    {
        public DiscoveredInstanceExport(Contract contract, IDictionary<string, object> metadata)
            : base(contract, metadata)
        {
        }

        protected override ExportDescriptor GetExportDescriptor(CompositeActivator partActivator)
        {
            return ExportDescriptor.Create(partActivator, Metadata);
        }

        public override DiscoveredExport CloseGenericExport(Type closedPartType, Type[] genericArguments)
        {
            var contract = Contract.ContractType.MakeGenericType(genericArguments);
            var newContract = new Contract(contract, Contract.Discriminator);
            return new DiscoveredInstanceExport(newContract, Metadata);
        }
    }
}
