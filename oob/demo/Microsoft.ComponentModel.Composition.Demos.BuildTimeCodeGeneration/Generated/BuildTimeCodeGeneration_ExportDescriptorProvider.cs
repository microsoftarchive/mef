using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildTimeCodeGeneration.Parts;

namespace BuildTimeCodeGeneration.Generated
{
    // This code is identical to what the container would generate at runtime.
    //
    class BuildTimeCodeGeneration_ExportDescriptorProvider : ExportDescriptorProvider
    {
        // The source will only be asked for parts once per exportKey, but it is the
        // source's responsibility to return the same part multiple times when that part
        // has more than one export (not shown here.)
        //
        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            if (exportKey.Discriminator != null)
                return NoExportDescriptors;

            if (exportKey.ContractType == typeof(RequestListener))
                return new[] { RequestListenerPart(exportKey, definitionAccessor) };

            if (exportKey.ContractType == typeof(ConsoleLog))
                return new[] { ConsoleLogPart(exportKey, definitionAccessor) };

            return NoExportDescriptors;
        }

        // Console log is a disposable singleton (no boundaries)
        // that exports itself under its own concrete type.
        // Note it uses the lock-free singleton instance optimisation,
        // but still defers creation to the sharing scope so that
        // synchronisation is correct. There are no locks required in user code.
        //
        ExportDescriptorPromise ConsoleLogPart(Contract contract, DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
                contract,
                typeof(ConsoleLog).Name,
                true,
                NoDependencies,
                _ =>
                {
                    var sharingId = LifetimeContext.AllocateSharingId();
                    object instance = null;

                    return ExportDescriptor.Create((c, o) =>
                    {
                        if (instance != null)
                            return instance;

                        var sharingScope = c.FindContextWithin(null);
                        instance = sharingScope.GetOrCreate(sharingId, o, (sc, so) =>
                        {
                            var result = new ConsoleLog();
                            c.AddBoundInstance(result);
                            return result;
                        });

                        return instance;
                    }, NoMetadata);
                });
        }

        // Non-shared part that exports itself and has a dependency on ConsoleLog.
        //
        ExportDescriptorPromise RequestListenerPart(Contract contract, DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
                contract,
                typeof(RequestListener).Name,
                false,
                () => new[] { definitionAccessor.ResolveRequiredDependency("log", new Contract(typeof(Lazy<ConsoleLog>)), true) },
                dependencies =>
                {
                    var logActivator = dependencies.Single().Target.GetDescriptor().Activator;
                    return ExportDescriptor.Create((c, o) =>
                    {
                        var log = (Lazy<ConsoleLog>)logActivator(c, o);
                        return new RequestListener(log);
                    }, NoMetadata);
                });
        }
    }
}
