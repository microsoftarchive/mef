// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Applied to an import for <see cref="ExportFactory{T}"/>, this attribute marks the
    /// boundary of a sharing scope. The <see cref="ExportLifetimeContext{T}"/> instances
    /// returned from the factory will be boundaries for sharing of components that are bounded
    /// by the listed boundary names.
    /// </summary>
    /// <example>
    /// [Import, SharingBoundary("HttpRequest")]
    /// public ExportFactory&lt;HttpRequestHandler&gt; HandlerFactory { get; set; }
    /// </example>
    /// <seealso cref="SharedAttribute" />
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    [MetadataAttribute]
    public class SharingBoundaryAttribute : Attribute
    {
        readonly string[] _sharingBoundaryNames;

        /// <summary>
        /// Construct a <see cref="SharingBoundaryAttribute"/> for the specified boundary names.
        /// </summary>
        /// <param name="sharingBoundaryNames">Boundaries implemented by the created <see cref="ExportLifetimeContext{T}"/>s.</param>
        public SharingBoundaryAttribute(params string[] sharingBoundaryNames)
        {
            if (sharingBoundaryNames == null) throw new ArgumentNullException("boundaries");

            _sharingBoundaryNames = sharingBoundaryNames;
        }

        /// <summary>
        /// Boundaries implemented by the created <see cref="ExportLifetimeContext{T}"/>s.
        /// </summary>
        public string[] SharingBoundaryNames { get { return _sharingBoundaryNames; } }
    }
}
