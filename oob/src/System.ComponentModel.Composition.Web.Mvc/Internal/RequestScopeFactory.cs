// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Web.Mvc.Internal
{
    /// <summary>
    /// Composition root for the MVC integration, do not call directly.
    /// </summary>
    [Export]
    class RequestScopeFactory
    {
        readonly ExportFactory<IExportProvider> _requestExportProviderFactory;

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="requestExportProviderFactory">For internal use.</param>
        [ImportingConstructor]
        public RequestScopeFactory(
            [SharingBoundary(Boundaries.HttpRequest, Boundaries.DataConsistency, Boundaries.UserIdentity)]
            ExportFactory<IExportProvider> requestExportProviderFactory)
        {
            _requestExportProviderFactory = requestExportProviderFactory;
        }

        internal ExportLifetimeContext<IExportProvider> BeginRequestScope()
        {
            return _requestExportProviderFactory.CreateExport();
        }
    }
}
