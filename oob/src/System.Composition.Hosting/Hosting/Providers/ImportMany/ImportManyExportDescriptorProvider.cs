// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Reflection;
using System.Composition.Hosting.Core;
using System.Composition.Hosting.Util;
using System.Composition.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace System.Composition.Hosting.Providers.ImportMany
{
    using System.Composition.Hosting.Properties;

    class ImportManyExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetImportManyDefinitionMethod = typeof(ImportManyExportDescriptorProvider).GetTypeInfo().GetDeclaredMethod("GetImportManyDescriptor");
        static readonly Type[] SupportedContractTypes = new[] { typeof(IList<>), typeof(ICollection<>), typeof(IEnumerable<>) };

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (!(contract.ContractType.IsArray ||
                  contract.ContractType.IsConstructedGenericType && SupportedContractTypes.Contains(contract.ContractType.GetGenericTypeDefinition())))
                return NoExportDescriptors;

            bool isImportMany;
            CompositionContract unwrapped;
            if (!contract.TryUnwrapMetadataConstraint(Constants.ImportManyImportMetadataConstraintName, out isImportMany, out unwrapped))
                return NoExportDescriptors;

            var elementType = contract.ContractType.IsArray ?
                contract.ContractType.GetElementType() :
                contract.ContractType.GenericTypeArguments[0];

            var elementContract = unwrapped.ChangeType(elementType);

            var gimd = GetImportManyDefinitionMethod.MakeGenericMethod(elementType);
            var gimdm = gimd.CreateStaticDelegate<Func<CompositionContract, CompositionContract, DependencyAccessor, object>>();
            return new[] { (ExportDescriptorPromise)gimdm(contract, elementContract, definitionAccessor) };
        }

        static ExportDescriptorPromise GetImportManyDescriptor<TElement>(CompositionContract importManyContract, CompositionContract elementContract, DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
                importManyContract,
                typeof(TElement[]).Name,
                false,
                () => definitionAccessor.ResolveDependencies("item", elementContract, true),
                d =>
                {
                    var dependentDescriptors = d
                        .Select(el => el.Target.GetDescriptor())
                        .ToArray();

                    return ExportDescriptor.Create((c, o) => dependentDescriptors.Select(e => (TElement)e.Activator(c, o)).ToArray(), NoMetadata);
                });
        }
    }
}
