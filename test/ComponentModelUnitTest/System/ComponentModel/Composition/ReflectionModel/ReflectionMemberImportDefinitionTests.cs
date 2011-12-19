// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    [TestClass]
    public class ReflectionMemberImportDefinitionTests
    {
        [TestMethod]
        public void Constructor()
        {
            PropertyInfo expectedMember = typeof(PublicImportsExpectingPublicExports).GetProperty("PublicImportPublicProperty");
            LazyMemberInfo expectedImportingMemberInfo = new LazyMemberInfo(expectedMember);
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata = new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Foo", typeof(object)) };
            IDictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["Key"] = "value";

            ReflectionMemberImportDefinition definition = new ReflectionMemberImportDefinition(
                expectedImportingMemberInfo, "Contract", (string)null, requiredMetadata, ImportCardinality.ZeroOrMore, true, false, CreationPolicy.NonShared, metadata,  null);

            Assert.AreEqual(expectedImportingMemberInfo, definition.ImportingLazyMember);

            Assert.AreEqual("Contract", definition.ContractName);
            Assert.AreSame(requiredMetadata, definition.RequiredMetadata);
            Assert.AreSame(metadata, definition.Metadata);
            Assert.AreEqual(CreationPolicy.NonShared, definition.RequiredCreationPolicy);
            Assert.AreEqual(true, definition.IsRecomposable);
            Assert.AreEqual(false, definition.IsPrerequisite);
            Assert.IsNull(((ICompositionElement)definition).Origin);
            Assert.IsNotNull(((ICompositionElement)definition).DisplayName);
            Assert.IsTrue(((ICompositionElement)definition).DisplayName.Contains(expectedMember.GetDisplayName()));
        }

        [TestMethod]
        public void Constructor_WithNullRequiredMetadata()
        {
            LazyMemberInfo member = CreateLazyMemberInfo();

            ReflectionMemberImportDefinition definition = new ReflectionMemberImportDefinition(
                member, "Contract", (string)null, null, ImportCardinality.ZeroOrMore, true, false, CreationPolicy.NonShared, null, null);

            Assert.IsNotNull(definition.RequiredMetadata);
            Assert.AreEqual(0, definition.RequiredMetadata.Count());
        }

        [TestMethod]
        public void SetDefinition_OriginIsSet()
        {
            LazyMemberInfo member = CreateLazyMemberInfo();
            var expectedPartDefinition = PartDefinitionFactory.CreateAttributed(typeof(object));
            ReflectionMemberImportDefinition definition = new ReflectionMemberImportDefinition(
                member, "Contract", (string)null, null, ImportCardinality.ZeroOrMore, true, false, CreationPolicy.NonShared, null, expectedPartDefinition);

            Assert.AreSame(expectedPartDefinition, ((ICompositionElement)definition).Origin);
        }

        private static LazyMemberInfo CreateLazyMemberInfo()
        {
            PropertyInfo expectedMember = typeof(PublicImportsExpectingPublicExports).GetProperty("PublicImportPublicProperty");
            return new LazyMemberInfo(expectedMember);
        }
    }
}
