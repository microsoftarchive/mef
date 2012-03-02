using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Extension
{
    public class DefaultExportDescriptorProvider : ExportDescriptorProvider
    {
        public override ExportDescriptorPromise[] GetExportDescriptors(Contract contract, DependencyAccessor descriptorAccessor)
        {
            if (contract.Discriminator is MetadataConstrainedDiscriminator)
                return NoExportDescriptors; // Not yet implemented

            var discriminator = contract.Discriminator as string;
            if (discriminator != null && discriminator.StartsWith(Constants.DefaultDiscriminatorPrefix))
                return NoExportDescriptors;

            var implementations = descriptorAccessor.ResolveDependencies("test for default", contract, false);
            if (implementations.Length != 0)
                return NoExportDescriptors;

            var defaultImplementationDiscriminator = Constants.DefaultDiscriminatorPrefix + (discriminator ?? "");
            var defaultImplementationContract = new Contract(contract.ContractType, defaultImplementationDiscriminator);

            Dependency defaultImplementation;
            if (!descriptorAccessor.TryResolveOptionalDependency("default", defaultImplementationContract, true, out defaultImplementation))
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                "Default Implementation",
                false,
                () => new[] { defaultImplementation },
                _ => {
                    var defaultDescriptor = defaultImplementation.Target.GetDescriptor();
                    return ExportDescriptor.Create((c, o) => defaultDescriptor.Activator(c, o), defaultDescriptor.Metadata);
                })};
        }
    }
}
