using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsExtensionDemo.Extension
{
    public class AppSettingsExportDescriptorProvider : ExportDescriptorProvider
    {
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            string key;
            CompositionContract unwrapped;

            if (!contract.TryUnwrapMetadataConstraint(Constants.SettingKey, out key, out unwrapped))
                return NoExportDescriptors;

            if (!unwrapped.Equals(new CompositionContract(unwrapped.ContractType)))
                return NoExportDescriptors;

            var value = ConfigurationManager.AppSettings.Get(key);
            if (value == null)
                return NoExportDescriptors;

            var converted = Convert.ChangeType(value, contract.ContractType);

            return new[] {
                new ExportDescriptorPromise(
                    contract,
                    "Application Configuration",
                    true,
                    NoDependencies,
                    _ => ExportDescriptor.Create((c, o) => converted, NoMetadata)) };
        }
    }
}
