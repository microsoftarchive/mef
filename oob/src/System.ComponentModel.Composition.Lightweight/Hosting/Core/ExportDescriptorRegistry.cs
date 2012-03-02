// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Util;
using System.Threading;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    class ExportDescriptorRegistry
    {
        readonly ExportDescriptorProvider[] _exportDescriptorProviders;

        IDictionary<Contract, ExportDescriptor[]> _partDefinitions = new Dictionary<Contract, ExportDescriptor[]>();

        public ExportDescriptorRegistry(ExportDescriptorProvider[] ExportDescriptorProviders)
        {
            _exportDescriptorProviders = ExportDescriptorProviders;
        }

        public bool TryGetSingleForExport(Contract exportKey, out ExportDescriptor defaultForExport)
        {
            ExportDescriptor[] allForExport;
            if (!_partDefinitions.TryGetValue(exportKey, out allForExport))
            {
                lock (_partDefinitions)
                {
                    if (!_partDefinitions.ContainsKey(exportKey))
                    {
                        var updatedDefinitions = new Dictionary<Contract, ExportDescriptor[]>(_partDefinitions);
                        var updateOperation = new ExportDescriptorRegistryUpdate(updatedDefinitions, _exportDescriptorProviders);
                        updateOperation.Execute(exportKey);

                        _partDefinitions = updatedDefinitions;
                        Thread.MemoryBarrier();
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
                throw new LightweightCompositionException(
                    string.Format("Multiple implementations of {0} found.", exportKey));

            defaultForExport = allForExport[0];
            return true;
        }
    }
}
