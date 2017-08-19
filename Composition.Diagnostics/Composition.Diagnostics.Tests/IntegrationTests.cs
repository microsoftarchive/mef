//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Microsoft.ComponentModel.Composition.Diagnostics.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        ComposablePartCatalog _catalog;
        CompositionContainer _container;
        CompositionInfo _analysis;

        public IntegrationTests()
        {
            _catalog = new TypeCatalog(
                typeof(Foo), typeof(Bar), typeof(Biff), typeof(MissingMeta),
                typeof(WrongCreationPolicy), typeof(WrongTypeIdentity));

            _container = new CompositionContainer(_catalog);

            _analysis = new CompositionInfo(_catalog, _container);
        }

        [TestMethod]
        public void FindsRootCauseOfRejection()
        {
            var fooInfo = _analysis.GetPartDefinitionInfo(typeof(Foo));

            var rootCause = fooInfo.FindPossibleRootCauses().Single();

            var barInfo = _analysis.GetPartDefinitionInfo(typeof(Bar));

            Assert.AreSame(barInfo, rootCause);
        }

        [TestMethod]
        public void FindsRootCauseOfRejectionWhenPartIsRootCause()
        {
            var barInfo = _analysis.GetPartDefinitionInfo(typeof(Bar));

            var rootCause = barInfo.FindPossibleRootCauses().Single();

            Assert.AreSame(barInfo, rootCause);
        }

        [TestMethod]
        public void FindsThreeUnsuitableExportsForBiff()
        {
            var barInfo = _analysis.GetPartDefinitionInfo(typeof(Bar));

            var biffImport = barInfo.ImportDefinitions.Single();

            Assert.AreEqual(3, biffImport.UnsuitableExportDefinitions.Count());
        }

        [TestMethod]
        public void DetectsMissingMetadataCase()
        {
            CheckReasonForBiffUnsuitability(
                typeof(MissingMeta),
                UnsuitableExportDefinitionReason.RequiredMetadata);
        }

        [TestMethod]
        public void DetectsIncompatibleCreationPolicyCase()
        {
            CheckReasonForBiffUnsuitability(
                typeof(WrongCreationPolicy),
                UnsuitableExportDefinitionReason.CreationPolicy);
        }

        [TestMethod]
        public void DetectsTypeIdentityCase()
        {
            CheckReasonForBiffUnsuitability(
                typeof(WrongTypeIdentity),
                UnsuitableExportDefinitionReason.TypeIdentity);
        }

        void CheckReasonForBiffUnsuitability(Type partType, UnsuitableExportDefinitionReason reason)
        {
            var barInfo = _analysis.GetPartDefinitionInfo(typeof(Bar));
            var unsuitablePart = _analysis.GetPartDefinitionInfo(partType);

            var issue = barInfo.ImportDefinitions.Single()
                .UnsuitableExportDefinitions
                .Where(ed => ed.PartDefinition == unsuitablePart)
                .Single()
                .Issues
                .Single();

            Assert.AreEqual(reason, issue.Reason);
        }

    }
}
