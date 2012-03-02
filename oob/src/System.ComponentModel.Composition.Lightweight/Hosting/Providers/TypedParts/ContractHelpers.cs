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

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts
{
    static class ContractHelpers
    {
        public static bool TryGetExplicitImportInfo(Type memberType, object[] attributes, out ImportInfo importInfo)
        {
            if (attributes.Any(a => a is ImportAttribute || a is ImportManyAttribute))
            {
                importInfo = GetImportInfo(memberType, attributes);
                return true;
            }

            importInfo = null;
            return false;
        }

        public static ImportInfo GetImportInfo(Type memberType, object[] attributes)
        {
            var exportKey = new Contract(memberType);
            IDictionary<string, object> importMetadata = null;
            var allowDefault = false;

            foreach (var attr in attributes)
            {
                var ia = attr as ImportAttribute;
                if (ia != null)
                {
                    exportKey = new Contract(ia.ContractType ?? memberType, ia.ContractName);
                    allowDefault = ia.AllowDefault;
                }
                else
                {
                    var ima = attr as ImportManyAttribute;
                    if (ima != null)
                    {
                        importMetadata = importMetadata ?? new Dictionary<string, object>();
                        importMetadata.Add(Constants.ImportManyImportMetadataConstraintName, true);
                        exportKey = new Contract(ima.ContractType ?? memberType, ima.ContractName);
                    }
                    else
                    {
                        var imca = attr as ImportMetadataConstraintAttribute;
                        if (imca != null)
                        {
                            importMetadata = importMetadata ?? new Dictionary<string, object>();
                            importMetadata.Add(imca.ConstraintName, imca.Value);
                        }
                        else
                        {
                            var attrType = attr.GetType();
                            // Note, we don't support ReflectionContext in this scenario
                            if (attrType.GetCustomAttribute<MetadataAttributeAttribute>(false) != null)
                            {
                                // We don't coalesce to collections here the way export metadata does
                                foreach (var prop in attrType
                                    .GetProperties()
                                    .Where(p => p.DeclaringType == attrType && p.CanRead))
                                {
                                    importMetadata = importMetadata ?? new Dictionary<string, object>();
                                    importMetadata.Add(prop.Name, prop.GetValue(attr, null));
                                }
                            }
                        }
                    }
                }
            }

            if (importMetadata != null)
            {
                exportKey = new Contract(exportKey.ContractType, new MetadataConstrainedDiscriminator(importMetadata, exportKey.Discriminator));
            }

            return new ImportInfo(exportKey, allowDefault);
        }

        public static bool IsShared(IDictionary<string, object> partMetadata)
        {
            return partMetadata.Contains(new KeyValuePair<string, object>(Constants.CreationPolicyPartMetadataName, CreationPolicy.Shared));
        }
    }
}
