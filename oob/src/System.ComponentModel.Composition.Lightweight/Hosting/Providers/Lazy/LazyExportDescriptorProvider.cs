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

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.Lazy
{
    class LazyExportDescriptorProvider : ExportDescriptorProvider
    {
        static readonly MethodInfo GetLazyDefinitionsMethod = typeof(LazyExportDescriptorProvider).GetMethod("GetLazyDefinitions", BindingFlags.NonPublic | BindingFlags.Static);

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            if (!exportKey.ContractType.IsGenericType || exportKey.ContractType.GetGenericTypeDefinition() != typeof(Lazy<>))
                return NoExportDescriptors;

            var gld = GetLazyDefinitionsMethod.MakeGenericMethod(exportKey.ContractType.GetGenericArguments()[0]);

            return (ExportDescriptorPromise[])gld.Invoke(null, new object[] { exportKey, definitionAccessor });
        }

        static ExportDescriptorPromise[] GetLazyDefinitions<T>(Contract contract, DependencyAccessor definitionAccessor)
        {
            return definitionAccessor.ResolveDependencies("value", new Contract(typeof(T), contract.Discriminator), false)
                .Select(d => new ExportDescriptorPromise(
                    contract,
                    typeof(Lazy<T>).Name,
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        return ExportDescriptor.Create((c, o) => new Lazy<T>(() => (T)CompositionOperation.Run(c, da)), dsc.Metadata);
                    }))
                .ToArray();
        }
    }
}
