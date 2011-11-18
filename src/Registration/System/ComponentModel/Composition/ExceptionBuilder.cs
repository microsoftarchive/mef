// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    internal static class ExceptionBuilder
    {
        public static ArgumentException Argument_ExpressionMustBeNew(string parameterName)
        {
            return CreateArgumentException(Strings.Argument_ExpressionMustBeNew, parameterName);
        }

        public static ArgumentException Argument_ExpressionMustBePropertyMember(string parameterName)
        {
            return CreateArgumentException(Strings.Argument_ExpressionMustBePropertyMember, parameterName);
        }

        private static ArgumentException CreateArgumentException(string message, string parameterName)
        {
            Assumes.NotNull(parameterName);

            return new ArgumentException(Format(message, parameterName), parameterName);
        }

        private static string Format(string format, params string[] arguments)
        {
            return String.Format(CultureInfo.CurrentCulture, format, arguments);
        }
    }
}
