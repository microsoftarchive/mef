// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.ComponentModelAttributeSupport
{
    public class ComponentModelMetadataViewProvider : ExportDescriptorProvider
    {
        const string MetadataViewProviderContractName = "MetadataViewProvider";
        static readonly MethodInfo GetMetadataViewProviderMethod = typeof(ComponentModelMetadataViewProvider).GetTypeInfo().GetDeclaredMethod("GetMetadataViewProvider");

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(
            CompositionContract contract,
            DependencyAccessor descriptorAccessor)
        {
            if (contract.ContractName != MetadataViewProviderContractName ||
                !contract.ContractType.IsConstructedGenericType ||
                contract.ContractType.GetGenericTypeDefinition() != typeof(Func<,>) ||
                contract.MetadataConstraints != null)
                return NoExportDescriptors;

            var providerArgs = contract.ContractType.GetGenericArguments();
            var argType = providerArgs[0];
            var viewType = providerArgs[1];
            if (!viewType.IsInterface ||
                argType != typeof(IDictionary<string,object>) ||
                viewType == typeof(IDictionary<string, object>))
                return NoExportDescriptors;

            var getViewMethod = GetMetadataViewProviderMethod.MakeGenericMethod(viewType);
            var viewProvider = getViewMethod.Invoke(null, null);

            return new[] { new ExportDescriptorPromise(
                contract,
                "Component Model Metadata View Provider",
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => viewProvider, NoMetadata)) };
        }

        static Func<IDictionary<string, object>, TMetadata> GetMetadataViewProvider<TMetadata>()
        {
            return m => AttributedModelServices.GetMetadataView<TMetadata>(m);
        }
    }
}
