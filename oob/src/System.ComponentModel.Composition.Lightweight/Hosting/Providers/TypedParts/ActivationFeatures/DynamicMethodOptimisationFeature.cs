// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Invoking a closure within a lambda expression is slower than invoking a regular delegate by
    /// quite a bit. This feature rewrites the closure that is formed by the call to GetOrCreate()
    /// by transforming the closure into a function of its arguments, then compiling the function
    /// and inserting it into the expression as a literal.
    /// </summary>
    class DynamicMethodOptimisationFeature : ActivationFeature
    {
        GetOrCreateLambdaRewriter _rewriter = new GetOrCreateLambdaRewriter();

        public override Expression RewriteActivator(
            Type partType,
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody,
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            if (!partMetadata.Contains(new KeyValuePair<string, object>(Constants.CreationPolicyPartMetadataName, CreationPolicy.Shared)))
                return activatorBody;

            return _rewriter.Visit(activatorBody);
        }
    }
}
