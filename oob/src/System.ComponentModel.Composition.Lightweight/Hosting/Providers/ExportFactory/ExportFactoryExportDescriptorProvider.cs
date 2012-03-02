// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.ExportFactory
{
    class ExportFactoryExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetExportFactoryDefinitionsMethod = typeof(ExportFactoryExportDescriptorProvider).GetMethod("GetExportFactoryDescriptors", BindingFlags.NonPublic | BindingFlags.Static);

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            if (!exportKey.ContractType.IsGenericType || exportKey.ContractType.GetGenericTypeDefinition() != typeof(ExportFactory<>))
                return NoExportDescriptors;

            var gld = GetExportFactoryDefinitionsMethod.MakeGenericMethod(exportKey.ContractType.GetGenericArguments()[0]);

            return (ExportDescriptorPromise[])gld.Invoke(null, new object[] { exportKey, definitionAccessor });
        }

        static ExportDescriptorPromise[] GetExportFactoryDescriptors<T>(Contract contract, DependencyAccessor definitionAccessor)
        {
            var innerDiscriminator = contract.Discriminator;
            var boundaries = new string[0];

            string[] specifiedBoundaries;
            object unwrappedDiscriminator;
            if (MetadataConstrainedDiscriminator.Unwrap(contract.Discriminator, Constants.SharingBoundaryImportMetadataConstraintName, out specifiedBoundaries, out unwrappedDiscriminator))
            {
                innerDiscriminator = unwrappedDiscriminator;
                boundaries = specifiedBoundaries;
            }

            return definitionAccessor.ResolveDependencies("product", new Contract(typeof(T), innerDiscriminator), false)
                .Select(d => new ExportDescriptorPromise(
                    contract,
                    typeof(ExportFactory<T>).Name,
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        return ExportDescriptor.Create((c, o) =>
                            {
                                return new ExportFactory<T>(() => {
                                    var lifetimeContext = new LifetimeContext(c, boundaries);
                                    return Tuple.Create<T, Action>((T)CompositionOperation.Run(lifetimeContext, da), lifetimeContext.Dispose);
                                });
                            },
                            dsc.Metadata);
                    }))
                .ToArray();
        }
    }
}
