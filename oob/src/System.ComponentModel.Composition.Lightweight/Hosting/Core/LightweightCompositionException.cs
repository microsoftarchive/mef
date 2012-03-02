// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// The exception type thrown when composition problems occur.
    /// Exception should be assumed to be fatal for the entire composition/container unless
    /// otherwise documented - no production code should throw this exception.
    /// </summary>
    [Serializable]
    public class LightweightCompositionException : Exception
    {
        /// <summary>
        /// Construct a <see cref="LightweightCompositionException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public LightweightCompositionException(string message)
            : base(message) { }

        /// <summary>
        /// Construct a <see cref="LightweightCompositionException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public LightweightCompositionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
