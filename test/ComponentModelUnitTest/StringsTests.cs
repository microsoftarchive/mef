// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Internal;
using System.Threading;
using System.Globalization;

namespace System
{
    [TestClass]
    public class StringsTests
    {
        [TestMethod]
        public void PropertiesAreInsyncWithResources()
        {
            var properties = GetStringProperties();

            Assert.IsTrue(properties.Length > 0, "Expected to find at least one string property in Strings.cs.");

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(null, (object[])null);

                Assert.IsNotNull(value, "Property '{0}' does not have an associated string in Strings.resx.", property.Name);
            }
        }

        private static PropertyInfo[] GetStringProperties()
        {
            PropertyInfo[] properties = typeof(Strings).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            return properties.Where(property => 
            {
                return !CanIgnore(property);

            }).ToArray();
        }

        private static bool CanIgnore(PropertyInfo property)
        {
            switch (property.Name)
            {
                case "Culture":
                case "ResourceManager":
                    return true;
            }

            return false;
        }
    }
}
