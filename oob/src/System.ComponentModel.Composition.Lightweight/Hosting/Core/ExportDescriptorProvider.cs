// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// A contributor to the composition.
    /// </summary>
    /// <remarks>Instances of this class are not required to be safe for concurrent access by
    /// multiple threads.</remarks>
    public abstract class ExportDescriptorProvider
    {
        /// <summary>
        /// Constant value provided so that subclasses can avoid creating additional duplicate values.
        /// </summary>
        protected static readonly ExportDescriptorPromise[] NoExportDescriptors = new ExportDescriptorPromise[0];

        /// <summary>
        /// Constant value provided so that subclasses can avoid creating additional duplicate values.
        /// </summary>
        protected static readonly IDictionary<string, object> NoMetadata = new Dictionary<string, object>();

        static readonly Dependency[] NoDependenciesValue = new Dependency[0];

        /// <summary>
        /// Constant value provided so that subclasses can avoid creating additional duplicate values.
        /// </summary>
        protected static readonly Func<Dependency[]> NoDependencies = () => NoDependenciesValue;

        /// <summary>
        /// Promise export descriptors for the specified export key.
        /// </summary>
        /// <param name="contract">The export key required by another component.</param>
        /// <param name="descriptorAccessor">Accesses the other export descriptors present in the composition.</param>
        /// <returns>Promises for new export descriptors.</returns>
        /// <remarks>
        /// A provider will only be queried once for each unique export key.
        /// The descriptor accessor can only be queried immediately if the descriptor being promised is an adapter, such as
        /// <see cref="Lazy{T}"/>; otherwise, dependencies should only be queried within execution of the function provided
        /// to the <see cref="ExportDescriptorPromise"/>. The actual descriptors provided should not close over or reference any
        /// aspect of the dependency/promise structure, as this should be able to be GC'ed.
        /// </remarks>
        public abstract ExportDescriptorPromise[] GetExportDescriptors(Contract contract, DependencyAccessor descriptorAccessor);
    }
}
