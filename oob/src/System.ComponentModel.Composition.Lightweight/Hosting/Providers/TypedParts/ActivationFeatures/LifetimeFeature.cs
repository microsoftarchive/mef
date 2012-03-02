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
using System.Threading;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Util;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies the activators of parts so that they a) get associated with the correct
    /// scope, and b) obtain their dependencies from the correct scope.
    /// </summary>
    class LifetimeFeature  : ActivationFeature
    {
        static readonly MethodInfo AddBoundInstanceMethod = typeof(LifetimeContext).GetMethod("AddBoundInstance");
        static readonly MethodInfo FindContextWithinMethod = typeof(LifetimeContext).GetMethod("FindContextWithin");
        static readonly MethodInfo GetOrCreate = typeof(LifetimeContext).GetMethod("GetOrCreate");

        public override Expression RewriteActivator(
            Type partType,
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody,
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            var isDisposable = typeof(IDisposable).IsAssignableFrom(partType);

            Expression result = activatorBody;
            Expression activationScope = compositionContextParameter;
            ParameterExpression calculatedScopeVar = null;

            var isShared = ContractHelpers.IsShared(partMetadata);
            if (isShared)
            {
                calculatedScopeVar = Expression.Variable(typeof(LifetimeContext), "ss");
                activationScope = calculatedScopeVar;
            }

            if (isDisposable)
            {
                var temp = Expression.Variable(typeof(object), "dt");
                result = Expression.Block(
                    new[] { temp },
                    Expression.Assign(temp, result),
                    Expression.Call(activationScope, AddBoundInstanceMethod, Expression.Convert(temp, typeof(IDisposable))),
                    temp);
            }

            if (isShared)
            {
                object sharingBoundary;
                if (!partMetadata.TryGetValue(Constants.SharedPartMetadataName, out sharingBoundary))
                    sharingBoundary = null;

                var calculatedScopeAssignment = Expression.Assign(calculatedScopeVar,
                    Expression.Call(compositionContextParameter,
                        FindContextWithinMethod,
                        Expression.Constant(sharingBoundary, typeof(string))));

                var scopeChangeRewriter = new ExchangeRewriter(compositionContextParameter, calculatedScopeVar);
                result = scopeChangeRewriter.Visit(result);

                var getOrCreateContextParam = Expression.Parameter(typeof(LifetimeContext), "cs");
                var contextParamRewriter = new ExchangeRewriter(calculatedScopeVar, getOrCreateContextParam);
                result = contextParamRewriter.Visit(result);

                var getOrCreateOperationParam = Expression.Parameter(typeof(CompositionOperation), "o");
                var operationParamRewriter = new ExchangeRewriter(operationParameter, getOrCreateOperationParam);
                result = operationParamRewriter.Visit(result);

                var creator = Expression.Lambda<CompositeActivator>(result, getOrCreateContextParam, getOrCreateOperationParam);
                var sharingKey = LifetimeContext.AllocateSharingId();

                result = Expression.Block(
                    new[] { calculatedScopeVar },
                    calculatedScopeAssignment,
                    Expression.Call(calculatedScopeVar, GetOrCreate, Expression.Constant(sharingKey), operationParameter, creator));
            }

            return result;
        }
    }
}
