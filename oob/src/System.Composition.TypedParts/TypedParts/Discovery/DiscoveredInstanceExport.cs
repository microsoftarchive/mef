// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Reflection;

namespace System.Composition.TypedParts.Discovery
{
    class DiscoveredInstanceExport : DiscoveredExport
    {
        public DiscoveredInstanceExport(CompositionContract contract, IDictionary<string, object> metadata)
            : base(contract, metadata)
        {
        }

        protected override ExportDescriptor GetExportDescriptor(CompositeActivator partActivator)
        {
            return ExportDescriptor.Create(partActivator, Metadata);
        }

        public override DiscoveredExport CloseGenericExport(TypeInfo closedPartType, Type[] genericArguments)
        {
            var closedContractType = Contract.ContractType.MakeGenericType(genericArguments);
            var newContract = Contract.ChangeType(closedContractType);
            return new DiscoveredInstanceExport(newContract, Metadata);
        }
    }
}
