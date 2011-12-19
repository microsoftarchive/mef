// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionContainerCycleTests
    {
        // There are nine possible scenarios that cause a part to have a dependency on another part, some of which
        // are legal and some not. For example, below, is not legal for a part, A, to have a prerequisite dependency
        // on a part, B, which has also has a prerequisite dependency on A. In contrast, however, it is legal for 
        // part A and B to have a non-prerequisite (Post) dependency on each other.
        // 
        // ------------------------------
        // |        |         B         |
        // |        | Pre | Post | None |
        // |--------|-----|------|------|
        // |   Pre  |  X  |    X |    v |
        // | A Post |  X  |    v |    v |
        // |   None |  v  |    v |    v |
        // ------------------------------
        //

        [TestMethod]
        public void APrerequisiteDependsOnBPrerequisite_ShouldThrowComposition()
        {
            AssertCycle(Dependency.Prerequisite,
                        Dependency.Prerequisite);
        }

        [TestMethod]
        public void APrerequisiteDependsOnBPost_ShouldThrowComposition()
        {
            AssertCycle(Dependency.Prerequisite,
                        Dependency.Post);
        }

        [TestMethod]
        public void APrerequisiteDependsOnBNone_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.Prerequisite,
                           Dependency.None);
        }

        [TestMethod]
        public void APostDependsOnBPrerequisite_ShouldThrowComposition()
        {
            AssertCycle(Dependency.Post,
                        Dependency.Prerequisite);
        }

        [TestMethod]
        public void APostDependsOnBPost_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.Post,
                           Dependency.Post);
        }

        [TestMethod]
        public void APostDependsOnBNone_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.Post,
                           Dependency.None);
        }

        [TestMethod]
        public void BPrerequisiteDependsOnANone_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.None,
                           Dependency.Prerequisite);
        }

        [TestMethod]
        public void BPostDependsOnANone_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.None,
                           Dependency.Post);
        }

        [TestMethod]
        public void ANoneWithBNone_ShouldNotThrow()
        {
            AssertNotCycle(Dependency.None,
                           Dependency.None);
        }

        private static void AssertCycle(Dependency partADependency, Dependency partBDependency)
        {
            var exportA = GetExport("A", partADependency, partBDependency);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                var value = exportA.Value;
            });

            var exportB = GetExport("B", partADependency, partBDependency);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                var value = exportB.Value;
            });
        }

        private static void AssertNotCycle(Dependency partADependency, Dependency partBDependency)
        {
            var exportA = GetExport("A", partADependency, partBDependency);
            var exportB = GetExport("B", partADependency, partBDependency);

            Assert.AreEqual("A", exportA.Value);
            Assert.AreEqual("B", exportB.Value);
        }

        private static Lazy<object, object> GetExport(string contractName, Dependency partADependency, Dependency partBDependency)
        {
            var container = GetContainer(partADependency, partBDependency);

            return container.GetExports(typeof(object), null, contractName).Single();
        }

        private static CompositionContainer GetContainer(Dependency partADependency, Dependency partBDependency)
        {
            var partA = CreatePartA(partADependency);
            var partB = CreatePartB(partBDependency);

            var catalog = CatalogFactory.Create(partA, partB);

            return ContainerFactory.Create(catalog);
        }

        private static ComposablePart CreatePartA(Dependency dependency)
        {
            return CreatePart(dependency, "A", "B");
        }

        private static ComposablePart CreatePartB(Dependency dependency)
        {
            return CreatePart(dependency, "B", "A");
        }

        private static ComposablePart CreatePart(Dependency dependency, string exportContractName, string importContractName)
        {
            ConcreteComposablePart part = new ConcreteComposablePart();
            part.AddExport(exportContractName, exportContractName);

            if (dependency != Dependency.None)
            {
                part.AddImport(importContractName, ImportCardinality.ExactlyOne, false, dependency == Dependency.Prerequisite);
            }

            return part;
        }

        private enum Dependency
        {
            Prerequisite,
            Post,
            None,
        }
    }
}
