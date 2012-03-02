// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Hosting.Util;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.Dictionaries
{
    class DictionaryExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetDictionaryDefinitionsMethod = typeof(DictionaryExportDescriptorProvider).GetMethod("GetDictionaryDefinition", BindingFlags.NonPublic | BindingFlags.Static);

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor descriptorAccessor)
        {
            if (!(exportKey.ContractType.IsGenericType && exportKey.ContractType.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                return NoExportDescriptors;

            object unwrappedDiscriminator;
            string keyByMetadataName;
            if (!MetadataConstrainedDiscriminator.Unwrap(exportKey.Discriminator, Constants.KeyByMetadataImportMetadataConstraintName, out keyByMetadataName, out unwrappedDiscriminator))
                return NoExportDescriptors;

            // RegistrationBuilder tacks [ImportMany] on enumerable dependencies; for this
            // scheme to work we need to keep the contracts separate, so until RB can be fixed
            // we explicitly ignore [ImportMany] dictionaries. Users need to place an [Import]
            // attribute onto dictionary imports when using RB.
            var importManyCheck = unwrappedDiscriminator as MetadataConstrainedDiscriminator;
            if (importManyCheck != null && importManyCheck.Metadata.ContainsKey(Constants.ImportManyImportMetadataConstraintName))
                return NoExportDescriptors;

            var args = exportKey.ContractType.GetGenericArguments();
            var keyType = args[0];
            var valueType = args[1];

            var gdd = GetDictionaryDefinitionsMethod.MakeGenericMethod(keyType, valueType);

            return new[] { (ExportDescriptorPromise)gdd.Invoke(null, new object[] { exportKey, unwrappedDiscriminator, descriptorAccessor, keyByMetadataName }) };
        }

        static ExportDescriptorPromise GetDictionaryDefinition<TKey, TValue>(Contract contract, object discriminator, DependencyAccessor definitionAccessor, string keyByMetadataName)
        {
            var itemValueKey = new Contract(typeof(TValue), discriminator);

            return new ExportDescriptorPromise(
                contract,
                typeof(IDictionary<TKey, TValue>).Name,
                false,
                () => definitionAccessor.ResolveDependencies("value", itemValueKey, true),
                deps => {
                    var items = deps.Select(d => Tuple.Create(d.Target.Origin, d.Target.GetDescriptor())).ToArray();
                    var isValidated = false;
                    return ExportDescriptor.Create((c, o) =>
                    {
                        if (!isValidated)
                        {
                            Validate<TKey>(items, keyByMetadataName);
                            isValidated = true;
                        }

                        return items.ToDictionary(
                            item => (TKey)item.Item2.Metadata[keyByMetadataName],
                            item => (TValue)item.Item2.Activator(c, o));
                    },
                    NoMetadata);
                });
        }

        static void Validate<TKey>(Tuple<string, ExportDescriptor>[] partsWithMatchedDescriptors, string keyByMetadataName)
        {
            var missing = partsWithMatchedDescriptors.Where(p => !p.Item2.Metadata.ContainsKey(keyByMetadataName)).ToArray();
            if (missing.Length != 0)
            {
                var problems = Formatters.ReadableQuotedList(missing.Select(p => p.Item1));
                var message = string.Format("The metadata '{0}' cannot be used as a dictionary import key because it is missing from exports on part(s) {1}.", keyByMetadataName, problems);
                throw new LightweightCompositionException(message);
            }

            var wrongType = partsWithMatchedDescriptors.Where(p => !(p.Item2.Metadata[keyByMetadataName] is TKey)).ToArray();
            if (wrongType.Length != 0)
            {
                var problems = Formatters.ReadableQuotedList(wrongType.Select(p => p.Item1));
                var message = string.Format("The metadata '{0}' cannot be used as a dictionary import key of type '{1}' because the value(s) supplied by {2} are of the wrong type.", keyByMetadataName, typeof(TKey).Name, problems);
                throw new LightweightCompositionException(message);
            }

            var firstDuplicated = partsWithMatchedDescriptors.GroupBy(p => (TKey)p.Item2.Metadata[keyByMetadataName]).Where(g => g.Count() > 1).FirstOrDefault();
            if (firstDuplicated != null)
            {
                var problems = Formatters.ReadableQuotedList(firstDuplicated.Select(p => p.Item1));
                var message = string.Format("The metadata '{0}' cannot be used as a dictionary import key because the value '{1}' is associated with exports from parts {2}.", keyByMetadataName, firstDuplicated.Key, problems);
                throw new LightweightCompositionException(message);
            }
        }
    }
}
