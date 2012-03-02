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
using System.Threading;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.Lazy
{
    class LazyWithMetadataExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetLazyDefinitionsMethod = typeof(LazyWithMetadataExportDescriptorProvider).GetMethod("GetLazyDefinitions", BindingFlags.NonPublic | BindingFlags.Static);

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            if (!exportKey.ContractType.IsGenericType || exportKey.ContractType.GetGenericTypeDefinition() != typeof(Lazy<,>))
                return NoExportDescriptors;

            var ga = exportKey.ContractType.GetGenericArguments();
            var gld = GetLazyDefinitionsMethod.MakeGenericMethod(ga[0], ga[1]);

            return (ExportDescriptorPromise[])gld.Invoke(null, new object[] { exportKey, definitionAccessor });
        }

        static ExportDescriptorPromise[] GetLazyDefinitions<T, TMetadata>(Contract contract, DependencyAccessor definitionAccessor)
        {
            return definitionAccessor.ResolveDependencies("value", new Contract(typeof(T), contract.Discriminator), false)
                .Select(d => new ExportDescriptorPromise(
                    contract,
                    typeof(Lazy<T, TMetadata>).Name,
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        var em = new Lazy<TMetadata>(() => AttributedModelServices.GetMetadataView<TMetadata>(dsc.Metadata), LazyThreadSafetyMode.PublicationOnly);
                        return ExportDescriptor.Create((c, o) => new Lazy<T, TMetadata>(() => (T)CompositionOperation.Run(c, da), em.Value), dsc.Metadata);
                    }))
                .ToArray();
        }
    }
}
