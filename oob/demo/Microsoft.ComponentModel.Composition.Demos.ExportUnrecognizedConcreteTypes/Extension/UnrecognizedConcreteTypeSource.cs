using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ComponentModel.Composition.Demos.ExportUnrecognizedConcreteTypes.Extension
{
    public class UnrecognizedConcreteTypeSource : ExportDescriptorProvider
    {
        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            if (exportKey.Discriminator != null ||
                exportKey.ContractType.IsAbstract ||
                !exportKey.ContractType.IsClass)
                return NoExportDescriptors;

            if (definitionAccessor.ResolveDependencies("test", exportKey, false).Length != 0)
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                exportKey,
                exportKey.ContractType.Name,
                false,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => Activator.CreateInstance(exportKey.ContractType), NoMetadata)) };
        }
    }
}
