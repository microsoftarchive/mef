// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies activators of parts that implement <see cref="IPartImportsSatisfiedNotification"/> so that
    /// their OnImportsSatisfied() method is correctly called.
    /// </summary>
    class PisnFeature : ActivationFeature
    {
        static readonly MethodInfo OnImportsSatisfiedMethod = typeof(IPartImportsSatisfiedNotification).GetMethod("OnImportsSatisfied");
        static readonly MethodInfo AddPostCompositionActionMethod = typeof(CompositionOperation).GetMethod("AddPostCompositionAction");

        public override Expression RewriteActivator(
            Type partType,
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody,
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            if (!typeof(IPartImportsSatisfiedNotification).IsAssignableFrom(partType))
                return activatorBody;

            var instanceVar = Expression.Variable(typeof(object), "ipisn");

            var action = Expression.Lambda<Action>(
                Expression.Call(
                    Expression.Convert(instanceVar, typeof(IPartImportsSatisfiedNotification)),
                    OnImportsSatisfiedMethod));

            return Expression.Block(
                new[] { instanceVar },
                Expression.Assign(instanceVar, activatorBody),
                Expression.Call(operationParameter, AddPostCompositionActionMethod, action),
                instanceVar);                    
        }
    }
}
