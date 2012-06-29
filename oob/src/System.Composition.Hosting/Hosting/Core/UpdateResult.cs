// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Internal;

namespace System.Composition.Hosting.Core
{
    // Update results ensure that providers can query reentrantly for the contract that
    // they are being queried for. The UpdateResult type keeps a list of remaining
    // providers, with providers being removed from the list before querying.
    class UpdateResult
    {
        static readonly ExportDescriptorPromise[] NoPromises = new ExportDescriptorPromise[0];

        readonly Queue<ExportDescriptorProvider> _remainingProviders;
        readonly IList<ExportDescriptorPromise> _providedDescriptors = new List<ExportDescriptorPromise>();
        ExportDescriptorPromise[] _results;

        public UpdateResult(IEnumerable<ExportDescriptorProvider> providers)
        {
            _remainingProviders = new Queue<ExportDescriptorProvider>(providers);
        }

        public bool TryDequeueNextProvider(out ExportDescriptorProvider provider)
        {
            if (_remainingProviders.Count == 0)
            {
                provider = null;
                return false;
            }

            provider = _remainingProviders.Dequeue();
            return true;
        }

        public void AddPromise(ExportDescriptorPromise promise)
        {
            _results = null;
            _providedDescriptors.Add(promise);
        }

        public ExportDescriptorPromise[] GetResults()
        {
            if (_results == null)
            {
                Assumes.IsTrue(_remainingProviders.Count == 0, "Providers remain to be queried.");

                if (_providedDescriptors.Count == 0)
                    _results = NoPromises;

                _results = _providedDescriptors.ToArray();
            }

            return _results;
        }
    }
}
