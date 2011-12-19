using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Registration;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;

namespace System.ComponentModel.Composition.RegistrationModel.ExportInterfacesContractExclusionTests
{
    public interface IContract1 { }

    public interface IContract2 { }

    public class ClassWithLifetimeConcerns : IContract1, IContract2, IDisposable, IPartImportsSatisfiedNotification
    {
        public void Dispose()
        {
        }

        public void OnImportsSatisfied()
        {
        }
    }


    [TestClass]
    public class ExportInterfacesContractExclusionTests
    {
        static readonly Type[] ContractInterfaces = new[] { typeof(IContract1), typeof(IContract2) };

        [TestMethod]
        public void WhenExportingInterfaces_NoPredicate_OnlyContractInterfacesAreExported()
        {
            var rb = new RegistrationBuilder();

            rb.ForType<ClassWithLifetimeConcerns>()
                .ExportInterfaces();

            var part = new TypeCatalog(new[] { typeof(ClassWithLifetimeConcerns) }, rb).Single();

            var exportedContracts = part.ExportDefinitions.Select(ed => ed.ContractName).ToArray();
            var expectedContracts = ContractInterfaces.Select(ci => AttributedModelServices.GetContractName(ci)).ToArray();

            CollectionAssert.AreEquivalent(expectedContracts, exportedContracts);
        }

        [TestMethod]
        public void WhenExportingInterfaces_PredicateSpecified_OnlyContractInterfacesAreSeenByThePredicate()
        {
            var seenInterfaces = new List<Type>();

            var rb = new RegistrationBuilder();
            
            rb.ForType<ClassWithLifetimeConcerns>()
                .ExportInterfaces(i => { seenInterfaces.Add(i); return true; });

            rb.MapType(typeof(ClassWithLifetimeConcerns).GetTypeInfo());

            var part = new TypeCatalog(new[] { typeof(ClassWithLifetimeConcerns) }, rb).Single();

            CollectionAssert.AreEquivalent(ContractInterfaces, seenInterfaces);
        }
    }
}
