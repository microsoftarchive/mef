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
    /// Allows <see cref="ExportDescriptorProvider"/>s to locate the dependencies they require.
    /// </summary>
    public abstract class DependencyAccessor
    {
        /// <summary>
        /// Get all definitions for a specified <see cref="Contract"/>.
        /// </summary>
        /// <param name="exportKey">The export key the definitions must supply.</param>
        /// <returns>The available promises for that export key.</returns>
        protected abstract ExportDescriptorPromise[] GetPromises(Contract exportKey);

        /// <summary>
        /// Resolve dependencies on all implementations of a contract.
        /// </summary>
        /// <param name="site">A tag describing the dependency site.</param>
        /// <param name="contract">The contract required by the site.</param>
        /// <param name="isPrerequisite">True if the dependency must be satisifed before corresponding exports can be retrieved; otherwise, false.</param>
        /// <returns>Dependencies for all implementations of the contact.</returns>
        public Dependency[] ResolveDependencies(object site, Contract contract, bool isPrerequisite)
        {
            var all = GetPromises(contract);
            var result = new Dependency[all.Length];
            for (var i = 0; i < all.Length; ++i)
                result[i] = Dependency.Satisfied(contract, all[i], isPrerequisite, site);
            return result;
        }

        /// <summary>
        /// Resolve a required dependency on exactly one implemenation of a contract.
        /// </summary>
        /// <param name="site">A tag describing the dependency site.</param>
        /// <param name="contract">The contract required by the site.</param>
        /// <param name="isPrerequisite">True if the dependency must be satisifed before corresponding exports can be retrieved; otherwise, false.</param>
        /// <returns>The dependency.</returns>
        public Dependency ResolveRequiredDependency(object site, Contract contract, bool isPrerequisite)
        {
            Dependency result;
            if (!TryResolveOptionalDependency(site, contract, isPrerequisite, out result))
                return Dependency.Missing(contract, site);

            return result;
        }

        /// <summary>
        /// Resolve an optional dependency on exactly one implemenation of a contract.
        /// </summary>
        /// <param name="site">A tag describing the dependency site.</param>
        /// <param name="contract">The contract required by the site.</param>
        /// <param name="isPrerequisite">True if the dependency must be satisifed before corresponding exports can be retrieved; otherwise, false.</param>
        /// <param name="dependency">The dependency, or null.</param>
        /// <returns>True if the dependency could be resolved; otherwise, false.</returns>
        public bool TryResolveOptionalDependency(object site, Contract contract, bool isPrerequisite, out Dependency dependency)
        {
            var all = GetPromises(contract);
            if (all.Length == 0)
            {
                dependency = null;
                return false;
            }

            if (all.Length != 1)
            {
                dependency = Dependency.Oversupplied(contract, all, site);
                return true;
            }

            dependency = Dependency.Satisfied(contract, all[0], isPrerequisite, site);
            return true;
        }
    }
}
