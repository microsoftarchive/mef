// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.Composition.Lightweight.Util;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// Default implementation for <see cref="IExportProvider"/>.
    /// </summary>
    /// <remarks>
    /// Currently there is an issue with this class/IExportProvider; these
    /// types together pull in quite a bit of hosting code and policy (Contract,
    /// ContracHelpers, MetadataConstrainedDiscriminator, ...) that, depending on
    /// assembly factoring, may be undesirable in the programming-model assemblies.
    /// </remarks>
    public abstract class ExportProviderBase : IExportProvider
    {
        /// <summary>
        /// Retrieve the single <paramref name="contract"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="contract">The contract to retrieve.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="LightweightCompositionException" />
        public abstract bool TryGetExport(Contract contract, out object export);

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The type of the export to retrieve.</typeparam>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="LightweightCompositionException" />
        public TExport GetExport<TExport>(object discriminator = null)
            where TExport : class
        {
            return (TExport)GetExport(typeof(TExport), discriminator);
        }

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="LightweightCompositionException" />
        public bool TryGetExport(Type exportType, object discriminator, out object export)
        {
            return TryGetExport(new Contract(exportType, discriminator), out export);
        }

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The type of the export to retrieve.</typeparam>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="LightweightCompositionException" />
        public bool TryGetExport<TExport>(object discriminator, out TExport export)
            where TExport : class
        {
            object untypedExport;
            if (!TryGetExport(typeof(TExport), discriminator, out untypedExport))
            {
                export = default(TExport);
                return false;
            }

            export = (TExport)untypedExport;
            return true;
        }

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="LightweightCompositionException" />
        public object GetExport(Type exportType, object discriminator = null)
        {
            return GetExport(new Contract(exportType, discriminator));
        }

        /// <summary>
        /// Retrieve the single <paramref name="contract"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="contract">The contract of the export to retrieve.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="LightweightCompositionException" />
        public object GetExport(Contract contract)
        {
            object export;
            if (!TryGetExport(contract, out export))
                throw new LightweightCompositionException(
                    string.Format("No export was found for the contract '{0}'.", contract));

            return export;
        }

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">The discriminator to apply when selecting the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="LightweightCompositionException" />
        public object[] GetExports(Type exportType, object discriminator = null)
        {
            var importManyDiscriminator = new MetadataConstrainedDiscriminator(
                new Dictionary<string, object> { { Constants.ImportManyImportMetadataConstraintName, true } },
                discriminator);

            var manyContract = new Contract(exportType.MakeArrayType(), importManyDiscriminator);

            return (object[])GetExport(manyContract);
        }

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The export type to retrieve.</typeparam>
        /// <returns>An instance of the export.</returns>
        /// <param name="discriminator">The discriminator to apply when selecting the export.</param>
        /// <exception cref="LightweightCompositionException" />
        public TExport[] GetExports<TExport>(object discriminator = null)
            where TExport : class
        {
            return (TExport[])GetExports(typeof(TExport), discriminator);
        }

        /// <summary>
        /// Set public properties decorated with the <see cref="ImportAttribute"/>.
        /// </summary>
        /// <remarks>Uses reflection, is slow - caching would help here.</remarks>
        /// <param name="conventions">Conventions to apply when satisfying loose imports; or null.</param>
        /// <param name="objectWithLooseImports">An object with decorated with import attributes.</param>
        public void SatisfyImports(object objectWithLooseImports, ReflectionContext conventions = null)
        {
            var ac = conventions == null ?
                (IAttributeContext)new DirectAttributeContext() :
                new ReflectionContextAttributeContext(conventions);

            foreach (var pi in objectWithLooseImports.GetType().GetProperties())
            {
                ImportInfo importInfo;
                if (ContractHelpers.TryGetExplicitImportInfo(pi.PropertyType, ac.GetDeclaredAttributes(pi), out importInfo))
                {
                    object value;
                    if (TryGetExport(importInfo.Contract, out value))
                    {
                        pi.SetValue(objectWithLooseImports, value);
                    }
                    else if (!importInfo.AllowDefault)
                    {
                        throw new LightweightCompositionException(string.Format(
                            "Missing dependency {0} on {1}.", pi.Name, objectWithLooseImports));
                    }
                }
            }

            var ipisn = objectWithLooseImports as IPartImportsSatisfiedNotification;
            if (ipisn != null)
            {
                ipisn.OnImportsSatisfied();
            }
        }
    }
}
