// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.Composition
{
    /// <summary>
    /// Can be imported by parts that wish to dynamically create instances of other parts.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public class ExportFactory<T>
    {
        private Func<Tuple<T, Action>> _exportLifetimeContextCreator;

        /// <summary>
        /// Construct an ExportFactory.
        /// </summary>
        /// <param name="exportCreator">Action invoked upon calls to the Create() method.</param>
        public ExportFactory(Func<Tuple<T, Action>> exportCreator)
        {
            if (exportCreator == null)
            {
                throw new ArgumentNullException("exportCreator");
            }

            this._exportLifetimeContextCreator = exportCreator;
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        public Export<T> CreateExport()
        {
            Tuple<T, Action> untypedLifetimeContext = this._exportLifetimeContextCreator.Invoke();
            return new Export<T>(untypedLifetimeContext.Item1, untypedLifetimeContext.Item2);
        }
    }
}
