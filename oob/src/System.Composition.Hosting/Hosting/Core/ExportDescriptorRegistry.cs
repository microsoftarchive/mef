// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Composition.Runtime;

namespace System.Composition.Hosting.Core
{
    class ExportDescriptorRegistry
    {
        readonly ExportDescriptorProvider[] _exportDescriptorProviders;

        IDictionary<CompositionContract, ExportDescriptor[]> _partDefinitions = new Dictionary<CompositionContract, ExportDescriptor[]>();

        public ExportDescriptorRegistry(ExportDescriptorProvider[] ExportDescriptorProviders)
        {
            _exportDescriptorProviders = ExportDescriptorProviders;
        }

        public bool TryGetSingleForExport(CompositionContract exportKey, out ExportDescriptor defaultForExport)
        {
            ExportDescriptor[] allForExport;
            if (!_partDefinitions.TryGetValue(exportKey, out allForExport))
            {
                lock (_partDefinitions)
                {
                    if (!_partDefinitions.ContainsKey(exportKey))
                    {
                        var updatedDefinitions = new Dictionary<CompositionContract, ExportDescriptor[]>(_partDefinitions);
                        var updateOperation = new ExportDescriptorRegistryUpdate(updatedDefinitions, _exportDescriptorProviders);
                        updateOperation.Execute(exportKey);

                        _partDefinitions = updatedDefinitions;
                        // Lock statement creates implicit memory barrier
                    }
                }

                allForExport = (ExportDescriptor[])_partDefinitions[exportKey];
            }

            if (allForExport.Length == 0)
            {
                defaultForExport = null;
                return false;
            }

            // This check is duplicated in the update process- the update operation will catch
            // cardinality violations in advance of this in all but a few very rare scenarios.
            if (allForExport.Length != 1)
                throw new CompositionFailedException(
                    string.Format("Multiple implementations of {0} found.", exportKey));

            defaultForExport = allForExport[0];
            return true;
        }
    }
}
