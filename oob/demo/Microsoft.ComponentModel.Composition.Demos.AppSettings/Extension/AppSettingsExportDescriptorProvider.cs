using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsExtensionDemo.Extension
{
    public class AppSettingsExportDescriptorProvider : ExportDescriptorProvider
    {
        public override ExportDescriptorPromise[] GetExportDescriptors(Contract exportKey, DependencyAccessor definitionAccessor)
        {
            string key;
            object unwrapped;

            if (!MetadataConstrainedDiscriminator.Unwrap(exportKey.Discriminator, Constants.SettingKey, out key, out unwrapped))
                return NoExportDescriptors;

            if (unwrapped != null)
                return NoExportDescriptors;

            var value = ConfigurationManager.AppSettings.Get(key);
            if (value == null)
                return NoExportDescriptors;

            var converted = Convert.ChangeType(value, exportKey.ContractType);

            return new[] {
                new ExportDescriptorPromise(
                    exportKey,
                    "Application Configuration",
                    true,
                    NoDependencies,
                    _ => ExportDescriptor.Create((c, o) => converted, NoMetadata)) };
        }
    }
}
