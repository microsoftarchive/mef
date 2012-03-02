// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Allows modification of the activators generated for typed parts.
    /// </summary>
    abstract class ActivationFeature
    {
        /// <summary>
        /// A constant shared for subclass use.
        /// </summary>
        protected static readonly Dependency[] NoDependencies = new Dependency[0];

        /// <summary>
        /// Participate in the activator creation process.
        /// </summary>
        /// <param name="partType">The part type being activated.</param>
        /// <param name="compositionContextParameter">Expression parameter for the composition context.</param>
        /// <param name="operationParameter">Expression parameter for the composition operation.</param>
        /// <param name="activatorBody">The activator body so far.</param>
        /// <param name="partMetadata">Metadata associated with the part being activated.</param>
        /// <param name="dependencies">Dependencies returned by a previous call to <see cref="GetDependencies"/>.</param>
        /// <returns>A new activator body, or the one already provided.</returns>
        public abstract Expression RewriteActivator(
            Type partType, 
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody, 
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies);

        /// <summary>
        /// Describe the dependencies required by this activation feature.
        /// </summary>
        /// <param name="partType">The part type being activated.</param>
        /// <param name="definitionAccessor">The definition accessor.</param>
        /// <returns>Dependencies.</returns>
        public virtual IEnumerable<Dependency> GetDependencies(Type partType, DependencyAccessor definitionAccessor)
        {
            return NoDependencies;
        }
    }
}
