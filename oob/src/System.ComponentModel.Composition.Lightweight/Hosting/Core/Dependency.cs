// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Util;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// Describes a dependency that a part must have in order to fulfill an
    /// <see cref="ExportDescriptorPromise"/>. Used by the composition engine during
    /// initialization to determine whether the composition can be made, and if not,
    /// what error to provide.
    /// </summary>
    public class Dependency
    {
        readonly ExportDescriptorPromise _target;
        readonly bool _isPrerequisite;
        readonly object _site;
        readonly Contract _contract;

        // Carrying some information to later use in error messages - 
        // it may be better to just store the message.
        readonly ExportDescriptorPromise[] _oversuppliedTargets;

        /// <summary>
        /// Construct a dependency on the specified target.
        /// </summary>
        /// <param name="target">The export descriptor promise from another part
        /// that this part is dependent on.</param>
        /// <param name="isPrerequisite">True if the dependency is a prerequisite
        /// that must be satisfied before any exports can be retrieved from the dependent
        /// part; otherwise, false.</param>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static Dependency Satisfied(Contract contract, ExportDescriptorPromise target, bool isPrerequisite, object site)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (site == null) throw new ArgumentNullException("site");
            if (contract == null) throw new ArgumentNullException("contract");

            return new Dependency(contract, target, isPrerequisite, site);
        }

        /// <summary>
        /// Construct a placeholder for a missing dependency. Note that this is different
        /// from an optional dependency - a missing dependency is an error.
        /// </summary>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static Dependency Missing(Contract contract, object site)
        {
            if (contract == null) throw new ArgumentNullException("contract");
            if (site == null) throw new ArgumentNullException("site");

            return new Dependency(contract, site);
        }

        /// <summary>
        /// Construct a placeholder for an "exactly one" dependency that cannot be
        /// configured because multiple target implementations exist.
        /// </summary>
        /// <param name="site">A marker used to identify the individual dependency among
        /// those on the dependent part.</param>
        /// <param name="targets">The targets found when expecting only one.</param>
        /// <param name="contract">The contract required by the dependency.</param>
        public static Dependency Oversupplied(Contract contract, ExportDescriptorPromise[] targets, object site)
        {
            if (targets == null) throw new ArgumentNullException("targets");
            if (site == null) throw new ArgumentNullException("site");
            if (contract == null) throw new ArgumentNullException("contract");

            return new Dependency(contract, targets, site);
        }

        Dependency(Contract contract, ExportDescriptorPromise target, bool isPrerequisite, object site)
        {
            _target = target;
            _isPrerequisite = isPrerequisite;
            _site = site;
            _contract = contract;
        }

        Dependency(Contract contract, object site)
        {
            _contract = contract;
            _site = site;
        }

        Dependency(Contract contract, ExportDescriptorPromise[] targets, object site)
        {
            _oversuppliedTargets = targets;
            _site = site;
            _contract = contract;
        }

        /// <summary>
        /// The export descriptor promise from another part
        /// that this part is dependent on.
        /// </summary>
        public ExportDescriptorPromise Target { get { return _target; } }

        /// <summary>
        /// True if the dependency is a prerequisite
        /// that must be satisfied before any exports can be retrieved from the dependent
        /// part; otherwise, false.
        /// </summary>
        public bool IsPrerequisite { get { return _isPrerequisite; } }

        /// <summary>
        /// A marker used to identify the individual dependency among
        /// those on the dependent part.
        /// </summary>
        public object Site { get { return _site; } }

        /// <summary>
        /// The contract required by the dependency.
        /// </summary>
        public Contract Contract { get { return _contract; } }

        /// <summary>
        /// Creates a human-readable explanation of the dependency.
        /// </summary>
        /// <returns>The dependency represented as a string.</returns>
        public override string ToString()
        {
            if (IsError)
                return Site.ToString();

            return string.Format("{0} on contract {1} supplied by {2}", Site, Target.Contract, Target.Origin);
        }

        internal bool IsError { get { return _target == null; } }

        internal void DescribeError(StringBuilder message)
        {
            if (!IsError)
                throw new InvalidOperationException("Dependency is not in an error state.");

            if (_oversuppliedTargets != null)
            {
                var list = Formatters.ReadableList(_oversuppliedTargets.Select(t => "'" + t.Origin + "'"));

                message.AppendFormat("Only one implementation of the contract '{0}' is allowed, but parts {1} export it", Contract, list);
            }
            else
            {
                message.AppendFormat("No export was found for the contract '{0}'", Contract);
            }
        }
    }
}
