using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.ComponentModel.Composition.Registration
{
    internal static class BuilderHelpers
    {
        private const string WiringRoot = ".ExplicitWiring++";

        public static string ConstructWiringName(Type type, string wiringName)
        {
            string name = AttributedModelServices.GetContractName(type);
            return name + WiringRoot + wiringName;
        }
    }
}
