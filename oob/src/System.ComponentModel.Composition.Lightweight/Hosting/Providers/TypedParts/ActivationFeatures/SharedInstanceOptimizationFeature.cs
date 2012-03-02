// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// For globally shared (singleton) parts, modifies the activator so that the instance
    /// is cached in a field once it is created. The feature implements what is effectively double-checked
    /// locking by deferring construction to the appropriate scope object.
    /// </summary>
    class SharedInstanceOptimizationFeature : ActivationFeature
    {
        static readonly FieldInfo HolderInstanceField = typeof(GlobalInstanceHolder).GetField("Instance");

        public override Expression RewriteActivator(
            Type partType,
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody, 
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            var isShared = partMetadata.Contains(new KeyValuePair<string, object>(Constants.CreationPolicyPartMetadataName, CreationPolicy.Shared));

            if (partMetadata.ContainsKey(Constants.SharedPartMetadataName) || !isShared)
                return activatorBody;

            var holder = Expression.Constant(new GlobalInstanceHolder());
            var instanceProperty = Expression.Field(holder, HolderInstanceField);

            return Expression.Condition(
                Expression.NotEqual(instanceProperty, Expression.Constant(null, typeof(object))),
                instanceProperty,
                Expression.Assign(instanceProperty, activatorBody));
        }
    }
}
