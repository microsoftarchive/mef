// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Util
{
    class ExchangeRewriter : ExpressionVisitor
    {
        private Expression _from;
        private Expression _to;

        public ExchangeRewriter(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _from)
                return _to;

            return base.Visit(node);
        }
    }
}
