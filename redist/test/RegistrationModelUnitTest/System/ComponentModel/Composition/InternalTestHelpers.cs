using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Registration
{
    internal static class Helpers
    {
        public static IEnumerable<Type> GetEnumerableOfTypes(params Type[] types)
        {
            return types;
        }

    }
}
