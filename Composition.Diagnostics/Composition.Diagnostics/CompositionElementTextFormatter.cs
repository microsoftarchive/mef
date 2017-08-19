//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Pretty-printer for objects involved in compositoin.
    /// </summary>
    public static class CompositionElementTextFormatter
    {
        /// <summary>
        /// Formats a composition element for display.
        /// </summary>
        /// <param name="compositionElement">Composition element, optionally
        /// implementing ICompositionElement.</param>
        /// <returns>String representation of compositionElement.</returns>
        public static string DisplayCompositionElement(object compositionElement)
        {
            return compositionElement is ICompositionElement ?
                ((ICompositionElement)compositionElement).DisplayName :
                compositionElement.ToString();
        }

        /// <summary>
        /// Formats extended information about a composition element for display.
        /// </summary>
        /// <param name="compositionElement">Composition element, optionally
        /// implementing ICompositionElement.</param>
        /// <returns>String description of compositionElement.</returns>
        public static string DescribeCompositionElement(object compositionElement)
        {
            var result = new StringBuilder();

            var nextInChain = compositionElement;

            while (nextInChain != null)
            {
                var nextIce = nextInChain as ICompositionElement;
                ICompositionElement parentIce = null;
                if (nextIce == null)
                {
                    result.Append(nextInChain.ToString());
                }
                else
                {
                    result.Append(nextIce.DisplayName);
                    parentIce = nextIce.Origin;
                }

                nextInChain = parentIce;

                if (nextInChain != null)
                    result.Append(" from: ");
            }

            return result.ToString();
        }
    }
}
