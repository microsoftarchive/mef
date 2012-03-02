// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.ComponentModel.Composition.Lightweight.Hosting.Util;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.ImportMany
{
    class ImportManyExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetImportManyDefinitionMethod = typeof(ImportManyExportDescriptorProvider).GetMethod("GetImportManyDescriptor", BindingFlags.NonPublic | BindingFlags.Static);
        static readonly Type[] SupportedContractTypes = new[] { typeof(IList<>), typeof(ICollection<>), typeof(IEnumerable<>) };

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract contract, DependencyAccessor definitionAccessor)
        {
            if (!(contract.ContractType.IsArray ||
                  contract.ContractType.IsGenericType && SupportedContractTypes.Contains(contract.ContractType.GetGenericTypeDefinition())))
                return NoExportDescriptors;

            bool isImportMany;
            object unwrappedDiscriminator;
            if (!MetadataConstrainedDiscriminator.Unwrap(contract.Discriminator, Constants.ImportManyImportMetadataConstraintName, out isImportMany, out unwrappedDiscriminator))
                return NoExportDescriptors;

            string keyToOrderBy;
            object orderUnwrappedDiscriminator;
            if (MetadataConstrainedDiscriminator.Unwrap(unwrappedDiscriminator, Constants.OrderByMetadataImportMetadataConstraintName, out keyToOrderBy, out orderUnwrappedDiscriminator))
                unwrappedDiscriminator = orderUnwrappedDiscriminator;

            var elementType = contract.ContractType.IsArray ?
                contract.ContractType.GetElementType() :
                contract.ContractType.GetGenericArguments()[0];

            var gimd = GetImportManyDefinitionMethod.MakeGenericMethod(elementType);

            return new[] { (ExportDescriptorPromise)gimd.Invoke(null, new object[] { contract, unwrappedDiscriminator, definitionAccessor, keyToOrderBy }) };
        }

        static ExportDescriptorPromise GetImportManyDescriptor<TElement>(Contract contract, object elementDiscriminator, DependencyAccessor definitionAccessor, string keyToOrderBy)
        {
            var elementContract = new Contract(typeof(TElement), elementDiscriminator);

            return new ExportDescriptorPromise(
                contract,
                typeof(TElement[]).Name,
                false,
                () => definitionAccessor.ResolveDependencies("item", elementContract, true),
                d =>
                {
                    var dependentDescriptors = (keyToOrderBy != null) ?
                        OrderDependentDescriptors(d, keyToOrderBy) :
                        d.Select(el => el.Target.GetDescriptor()).ToArray();

                    return ExportDescriptor.Create((c, o) => dependentDescriptors.Select(e => (TElement)e.Activator(c, o)).ToArray(), NoMetadata);
                });
        }

        static ExportDescriptor[] OrderDependentDescriptors(Dependency[] dependentDescriptors, string keyToOrderBy)
        {
            var targets = dependentDescriptors.Select(d => d.Target).ToArray();
            var missing = targets.Where(t => !t.GetDescriptor().Metadata.ContainsKey(keyToOrderBy) ||
                                             t.GetDescriptor().Metadata[keyToOrderBy] == null).ToArray();
            if (missing.Length != 0)
            {
                var origins = Formatters.ReadableQuotedList(missing.Select(m => m.Origin));
                var message = string.Format("The metadata '{0}' cannot be used for ordering because it is missing from exports on part(s) {1}.", keyToOrderBy, origins);
                throw new LightweightCompositionException(message);
            }

            return targets.Select(t => t.GetDescriptor()).OrderBy(d => d.Metadata[keyToOrderBy]).ToArray();
        }
    }
}
