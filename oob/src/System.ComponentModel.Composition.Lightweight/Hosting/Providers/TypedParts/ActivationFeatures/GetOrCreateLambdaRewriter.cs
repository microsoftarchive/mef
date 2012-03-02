// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    class GetOrCreateLambdaRewriter : ExpressionVisitor
    {
        static readonly MethodInfo GetOrCreateMethod = typeof(LifetimeContext).GetMethod("GetOrCreate");

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == GetOrCreateMethod)
            {
                var lambda = (Expression<CompositeActivator>)node.Arguments[2];
                var creator = lambda.Compile();
                return Expression.Call(
                    node.Object,
                    node.Method,
                    node.Arguments[0],
                    node.Arguments[1],
                    Expression.Constant(creator, typeof(CompositeActivator)));
            }

            return base.VisitMethodCall(node);
        }
    }
}
