// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.Composition
{
    /// <summary>
    /// An ExportFactory that provides metadata describing the created exports.
    /// </summary>
    /// <typeparam name="T">The contract type being created.</typeparam>
    /// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
    public class ExportFactory<T, TMetadata> : ExportFactory<T>
    {
        private readonly TMetadata _metadata;

        /// <summary>
        /// Construct an ExportFactory.
        /// </summary>
        /// <param name="exportCreator">Action invoked upon calls to the Create() method.</param>
        /// <param name="metadata">The metadata associated with the export.</param>
        public ExportFactory(Func<Tuple<T, Action>> exportCreator, TMetadata metadata)
            : base(exportCreator)
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// The metadata associated with the export.
        /// </summary>
        public TMetadata Metadata
        {
            get { return this._metadata; }
        }
    }
}

