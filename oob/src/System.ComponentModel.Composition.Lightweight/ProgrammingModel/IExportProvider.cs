// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Implemented by types that can provide exports from the composition graph.
    /// </summary>
    public interface IExportProvider
    {
        /// <summary>
        /// Retrieve the single <paramref name="contract"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="contract">The contract to retrieve.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        bool TryGetExport(Contract contract, out object export);

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The type of the export to retrieve.</typeparam>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        TExport GetExport<TExport>(object discriminator = null)
            where TExport : class;

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        bool TryGetExport(Type exportType, object discriminator, out object export);

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The type of the export to retrieve.</typeparam>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <param name="export">The export if available, otherwise, null.</param>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        bool TryGetExport<TExport>(object discriminator, out TExport export)
            where TExport : class;

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">Optionally, a discriminator that constrains the selection of the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        object GetExport(Type exportType, object discriminator = null);

        /// <summary>
        /// Retrieve the single <paramref name="contract"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="contract">The contract of the export to retrieve.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        object GetExport(Contract contract);

        /// <summary>
        /// Retrieve the single <paramref name="exportType"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <param name="exportType">The type of the export to retrieve.</param>
        /// <param name="discriminator">The discriminator to apply when selecting the export.</param>
        /// <returns>An instance of the export.</returns>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        object[] GetExports(Type exportType, object discriminator = null);

        /// <summary>
        /// Retrieve the single <typeparamref name="TExport"/> instance from the
        /// <see cref="IExportProvider"/>.
        /// </summary>
        /// <typeparam name="TExport">The export type to retrieve.</typeparam>
        /// <returns>An instance of the export.</returns>
        /// <param name="discriminator">The discriminator to apply when selecting the export.</param>
        /// <exception cref="System.ComponentModel.Composition.Lightweight.Hosting.Core.LightweightCompositionException" />
        TExport[] GetExports<TExport>(object discriminator = null)
            where TExport : class;

        /// <summary>
        /// Set public properties decorated with the <see cref="ImportAttribute"/>.
        /// </summary>
        /// <remarks>Uses reflection, is slow - caching would help here.</remarks>
        /// <param name="conventions">Conventions to apply when satisfying loose imports; or null.</param>
        /// <param name="objectWithLooseImports">An object with decorated with import attributes.</param>
        void SatisfyImports(object objectWithLooseImports, ReflectionContext conventions = null);
    }
}
