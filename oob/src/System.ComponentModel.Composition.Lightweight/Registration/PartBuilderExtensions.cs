// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Registration
{
    /// <summary>
    /// Helper methods for <see cref="PartBuilder"/>.
    /// </summary>
    public static class PartBuilderExtensions
    {
        /// <summary>
        /// Mark the part as shared.
        /// </summary>
        /// <typeparam name="T">Part bilder type.</typeparam>
        /// <param name="partBuilder">The part builder.</param>
        /// <returns>The part builder.</returns>
        public static T Shared<T>(this T partBuilder) where T : PartBuilder
        {
            if (partBuilder == null) throw new ArgumentNullException("partBuilder");
            partBuilder.SetCreationPolicy(CreationPolicy.Shared);
            return partBuilder;
        }

        /// <summary>
        /// Mark the part as constrained to a sharing boundary.
        /// </summary>
        /// <param name="boundary">Sharing boundary.</param>
        /// <typeparam name="T">Part bilder type.</typeparam>
        /// <param name="partBuilder">The part builder.</param>
        /// <returns>The part builder.</returns>
        public static T Shared<T>(this T partBuilder, string boundary) where T : PartBuilder
        {
            if (partBuilder == null) throw new ArgumentNullException("partBuilder");
            if (boundary == null) throw new ArgumentNullException("boundary");

            Shared(partBuilder);
            partBuilder.AddMetadata(Constants.SharedPartMetadataName, boundary);
            return partBuilder;
        }
    }
}
