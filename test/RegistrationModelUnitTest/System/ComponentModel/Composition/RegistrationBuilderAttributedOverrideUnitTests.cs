using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Registration
{
    [TestClass]
    public class RegistrationBuilderAttributedOverrideUnitTests
    {
        public interface IContractA { }
        public interface IContractB { }

        public class AB : IContractA, IContractB { }

        static class ContractNames
        {
            public const string ContractX = "X";
            public const string ContractY = "Y";
        }

        static class MetadataKeys
        {
            public const string MetadataKeyP = "P";
            public const string MetadataKeyQ = "Q";
        }

        static class MetadataValues
        {
            public const string MetadataValueN = "N";
            public const string MetadataValueO = "O";
        }

        // Flattened so that we can be sure nothing funky is going on in the base type

        static void AssertHasDeclaredAttributesUnderConvention<TSource>(RegistrationBuilder convention)
        {
            AssertHasAttributesUnderConvention<TSource>(convention, typeof(TSource).GetCustomAttributes(true));
        }

        static void AssertHasAttributesUnderConvention<TSource>(RegistrationBuilder convention, IEnumerable<object> expected)
        {
            var mapped = convention.MapType(typeof(TSource).GetTypeInfo());
            var applied = mapped.GetCustomAttributes(true);

            // Was: CollectionAssert.AreEquivalent(expected, applied) - output is not much good.
            AssertEquivalentAttributes(expected, applied);
        }

        static PropertyInfo GetPropertyFromAccessor<T>(Expression<Func<T, object>> property)
        {
            return (PropertyInfo)((MemberExpression)property.Body).Member;
        }

        static void AssertHasDeclaredAttributesUnderConvention<TSource>(Expression<Func<TSource, object>> property, RegistrationBuilder convention)
        {
            var pi = GetPropertyFromAccessor(property);
            AssertHasAttributesUnderConvention<TSource>(property, convention, pi.GetCustomAttributes(true));
        }

        static void AssertHasAttributesUnderConvention<TSource>(Expression<Func<TSource,object>> property, RegistrationBuilder convention, IEnumerable<object> expected)
        {
            var mapped = convention.MapType(typeof(TSource).GetTypeInfo());
            var pi = GetPropertyFromAccessor(property);
            var applied = mapped.GetProperty(pi.Name).GetCustomAttributes(true);

            // Was: CollectionAssert.AreEquivalent(expected, applied) - output is not much good.
            AssertEquivalentAttributes(expected, applied);
        }

        static void AssertEquivalentAttributes(IEnumerable<object> expected, IEnumerable<object> applied)
        {
            var expectedRemaining = expected.ToList();
            var unexpected = new List<object>();
            foreach (var appl in applied)
            {
                var matching = expectedRemaining.FirstOrDefault(e => object.Equals(e, appl));
                if (matching == null)
                    unexpected.Add(appl);
                else
                    expectedRemaining.Remove(matching);
            }

            var failures = new List<string>();
            if (expectedRemaining.Any())
                failures.Add("Expected attributes: " + string.Join(", ", expectedRemaining));
            if (unexpected.Any())
                failures.Add("Did not expect attributes: " + string.Join(", ", unexpected));

            if (failures.Any())
                throw new AssertFailedException(string.Join(Environment.NewLine, failures));
        }

        // This set of tests is for exports at the class declaration level

        static RegistrationBuilder ConfigureExportInterfaceConvention<TPart>()
        {
            var convention = new RegistrationBuilder();

            convention.ForType<TPart>()
                 .Export(eb => eb.AsContractType<IContractA>()
                     .AddMetadata(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN));

            return convention;
        }

        static object[] ExportInterfaceConventionAttributes = new object[] {
            new ExportAttribute(typeof(IContractA)),
            new ExportMetadataAttribute(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN)
        };

        public class NoClassDeclarationOverrides : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_NoOverrides_ConventionApplied()
        {
            var convention = ConfigureExportInterfaceConvention<NoClassDeclarationOverrides>();

            AssertHasAttributesUnderConvention<NoClassDeclarationOverrides>(convention, ExportInterfaceConventionAttributes);
        }

        [Export(typeof(IContractB))]
        public class ExportContractBClassDeclarationOverride : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_ExportAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportInterfaceConvention<ExportContractBClassDeclarationOverride>();

            AssertHasDeclaredAttributesUnderConvention<ExportContractBClassDeclarationOverride>(convention);
        }

        [ExportMetadata(MetadataKeys.MetadataKeyQ, MetadataValues.MetadataValueO)]
        public class ExportJustMetadataOverride : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_JustMetadataOverride_ConventionIgnored()
        {
            var convention = ConfigureExportInterfaceConvention<ExportJustMetadataOverride>();

            AssertHasDeclaredAttributesUnderConvention<ExportJustMetadataOverride>(convention);
        }

        [InheritedExport(typeof(IContractA))]
        public class InheritedExportContractAClassDeclarationOverride : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_InheritedExportAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportInterfaceConvention<InheritedExportContractAClassDeclarationOverride>();

            AssertHasDeclaredAttributesUnderConvention<InheritedExportContractAClassDeclarationOverride>(convention);
        }

        [InheritedExport(typeof(IContractA))]
        public class BaseWithInheritedExport { }

        public class InheritedExportOnBaseClassDeclaration : BaseWithInheritedExport, IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_InheritedExportOnBaseClassDeclaration_ConventionApplied()
        {
            var convention = ConfigureExportInterfaceConvention<InheritedExportOnBaseClassDeclaration>();

            AssertHasAttributesUnderConvention<InheritedExportOnBaseClassDeclaration>(convention,
                ExportInterfaceConventionAttributes.Concat(new object[] { new InheritedExportAttribute(typeof(IContractA)) }));
        }

        public class CustomExportAttribute : ExportAttribute { }

        [CustomExport]
        public class CustomExportClassDeclarationOverride : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_CustomExportAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportInterfaceConvention<CustomExportClassDeclarationOverride>();

            AssertHasDeclaredAttributesUnderConvention<CustomExportClassDeclarationOverride>(convention);
        }

        [MetadataAttribute]
        public class CustomMetadataAttribute : Attribute { public string Z { get { return "Z"; } } }

        [CustomMetadata]
        public class CustomMetadataClassDeclarationOverride : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_CustomMetadataAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportInterfaceConvention<CustomMetadataClassDeclarationOverride>();

            AssertHasDeclaredAttributesUnderConvention<CustomMetadataClassDeclarationOverride>(convention);
        }
        
        [PartCreationPolicy(CreationPolicy.NonShared),
         PartMetadata(MetadataKeys.MetadataKeyQ, MetadataValues.MetadataValueO),
         PartNotDiscoverable]
        public class NonExportClassDeclarationAttributes : IContractA, IContractB { }

        [TestMethod]
        public void ExportInterfaceConvention_NonExportClassDeclarationAttributes_ConventionApplied()
        {
            var convention = ConfigureExportInterfaceConvention<NonExportClassDeclarationAttributes>();

            var unionOfConventionAndDeclared = typeof(NonExportClassDeclarationAttributes)
                .GetCustomAttributes(true)
                .Concat(ExportInterfaceConventionAttributes)
                .ToArray();

            AssertHasAttributesUnderConvention<NonExportClassDeclarationAttributes>(convention, unionOfConventionAndDeclared);
        }

        public class ExportAtProperty
        {
            [Export]
            public IContractA A { get; set; }
        }

        [TestMethod]
        public void ExportInterfacesConvention_UnrelatedExportOnProperty_ConventionApplied()
        {
            var convention = ConfigureExportInterfaceConvention<ExportAtProperty>();
            AssertHasAttributesUnderConvention<ExportAtProperty>(convention, ExportInterfaceConventionAttributes);
        }

        // This set of tests is for exports at the property level

        static RegistrationBuilder ConfigureExportPropertyConvention<TPart>(Expression<Func<TPart, object>> property)
        {
            var convention = new RegistrationBuilder();

            convention.ForType<TPart>()
                 .ExportProperty(property, eb => eb.AsContractType<IContractA>()
                     .AddMetadata(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN));

            return convention;
        }

        static object[] ExportPropertyConventionAttributes = new object[] {
            new ExportAttribute(typeof(IContractA)),
            new ExportMetadataAttribute(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN)
        };

        public class NoPropertyDeclarationOverrides
        {
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_NoOverrides_ConventionApplied()
        {
            var convention = ConfigureExportPropertyConvention<NoPropertyDeclarationOverrides>(t => t.AB);

            AssertHasAttributesUnderConvention<NoPropertyDeclarationOverrides>(t => t.AB, convention, ExportPropertyConventionAttributes);
        }

        public class ExportContractBPropertyDeclarationOverride
        {
            [Export(typeof(IContractB))]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_ExportAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportPropertyConvention<ExportContractBPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<ExportContractBPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class ExportJustMetadataPropertyDeclarationOverride
        {
            [ExportMetadata(MetadataKeys.MetadataKeyQ, MetadataValues.MetadataValueO)]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_JustMetadataOverride_ConventionIgnored()
        {
            var convention = ConfigureExportPropertyConvention<ExportJustMetadataPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<ExportJustMetadataPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class CustomExportPropertyDeclarationOverride
        {
            [CustomExport]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_CustomExportAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportPropertyConvention<CustomExportPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<CustomExportPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class CustomMetadataPropertyDeclarationOverride
        {
            [CustomMetadata]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_CustomMetadataAttribute_ConventionIgnored()
        {
            var convention = ConfigureExportPropertyConvention<CustomMetadataPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<CustomMetadataPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class NonExportPropertyDeclarationAttributes
        {
            [Import, ImportMany]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ExportPropertyConvention_NonExportPropertyDeclarationAttributes_ConventionApplied()
        {
            var convention = ConfigureExportPropertyConvention<NonExportPropertyDeclarationAttributes>(t => t.AB);

            var unionOfConventionAndDeclared = typeof(NonExportPropertyDeclarationAttributes)
                .GetProperty("AB")
                .GetCustomAttributes(true)
                .Concat(ExportPropertyConventionAttributes)
                .ToArray();

            AssertHasAttributesUnderConvention<NonExportPropertyDeclarationAttributes>(t => t.AB, convention, unionOfConventionAndDeclared);
        }

        // This set of tests is for imports at the property level
        static RegistrationBuilder ConfigureImportPropertyConvention<TPart>(Expression<Func<TPart, object>> property)
        {
            var convention = new RegistrationBuilder();

            convention.ForType<TPart>()
                 .ImportProperty(property, ib => ib.AsMany(false).AsContractName(ContractNames.ContractX).AsContractType<AB>());

            return convention;
        }

        static object[] ImportPropertyConventionAttributes = new object[] { new ImportAttribute(ContractNames.ContractX, typeof(AB)) };

        [TestMethod]
        public void ImportPropertyConvention_NoOverrides_ConventionApplied()
        {
            var convention = ConfigureImportPropertyConvention<NoPropertyDeclarationOverrides>(t => t.AB);

            AssertHasAttributesUnderConvention<NoPropertyDeclarationOverrides>(t => t.AB, convention, ImportPropertyConventionAttributes);
        }

        public class ImportContractYPropertyDeclarationOverride
        {
            [Import(ContractNames.ContractY)]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ImportPropertyConvention_ImportAttribute_ConventionIgnored()
        {
            var convention = ConfigureImportPropertyConvention<ImportContractYPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<ImportContractYPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class ImportManyPropertyDeclarationOverride
        {
            [ImportMany]
            public AB[] AB { get; set; }
        }

        [TestMethod]
        public void ImportPropertyConvention_ImportManyAttribute_ConventionIgnored()
        {
            var convention = ConfigureImportPropertyConvention<ImportManyPropertyDeclarationOverride>(t => t.AB);

            AssertHasDeclaredAttributesUnderConvention<ImportManyPropertyDeclarationOverride>(t => t.AB, convention);
        }

        public class NonImportPropertyDeclarationAttributes
        {
            [Export, ExportMetadata(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN)]
            public AB AB { get; set; }
        }

        [TestMethod]
        public void ImportPropertyConvention_NonImportPropertyDeclarationAttributes_ConventionApplied()
        {
            var convention = ConfigureImportPropertyConvention<NonImportPropertyDeclarationAttributes>(t => t.AB);

            var unionOfConventionAndDeclared = typeof(NonImportPropertyDeclarationAttributes)
                .GetProperty("AB")
                .GetCustomAttributes(true)
                .Concat(ImportPropertyConventionAttributes)
                .ToArray();

            AssertHasAttributesUnderConvention<NonImportPropertyDeclarationAttributes>(t => t.AB, convention, unionOfConventionAndDeclared);
        }
        
        // The following test is for importing constructors

        class TwoConstructorsWithOverride
        {
            [ImportingConstructor]
            public TwoConstructorsWithOverride(IContractA a) { }

            public TwoConstructorsWithOverride(IContractA a, IContractB b) { }
        }

        [TestMethod]
        public void ConstructorConvention_OverrideOnDeclaration_ConventionIgnored()
        {
            var rb = new RegistrationBuilder();
            rb.ForType<TwoConstructorsWithOverride>()
                .SelectConstructor(pi => new TwoConstructorsWithOverride(pi.Import<IContractA>(), pi.Import<IContractB>()));

            var mapped = rb.MapType(typeof(TwoConstructorsWithOverride).GetTypeInfo());
            
            var conventional = mapped.GetConstructor(new[] { rb.MapType(typeof(IContractA).GetTypeInfo()), rb.MapType(typeof(IContractB).GetTypeInfo()) });
            var conventionalAttrs = conventional.GetCustomAttributes(true);
            Assert.IsFalse(conventionalAttrs.Any());
            
            var overridden = mapped.GetConstructor(new[] { rb.MapType(typeof(IContractA).GetTypeInfo()) });
            var overriddenAttr = overridden.GetCustomAttributes(true).Single();
            Assert.AreEqual(new ImportingConstructorAttribute(), overriddenAttr);
        }

        // Tests follow for constructor parameters

        static RegistrationBuilder ConfigureImportConstructorParameterConvention<TPart>()
        {
            var convention = new RegistrationBuilder();

            convention.ForType<TPart>()
                 .SelectConstructor(cis => cis.Single(), (ci, ib) => ib.AsMany(false).AsContractName(ContractNames.ContractX).AsContractType<IContractA>());

            return convention;
        }

        static object[] ImportParameterConventionAttributes = new object[] { new ImportAttribute(ContractNames.ContractX, typeof(IContractA)) };

        class NoConstructorParameterOverrides
        {
            public NoConstructorParameterOverrides(IContractA a) { }
        }

        [TestMethod]
        public void ConstructorParameterConvention_NoOverride_ConventionApplied()
        {
            var convention = ConfigureImportConstructorParameterConvention<NoConstructorParameterOverrides>();
            var mapped = convention.MapType(typeof(NoConstructorParameterOverrides).GetTypeInfo());
            var pi = mapped.GetConstructors().Single().GetParameters().Single();
            var actual = pi.GetCustomAttributes(true);
            AssertEquivalentAttributes(ImportParameterConventionAttributes, actual);
        }

        class ConstructorParameterImportContractX
        {
            public ConstructorParameterImportContractX([Import(ContractNames.ContractX)] IContractA a) { }
        }

        [TestMethod]
        public void ConstructorParameterConvention_ImportOnDeclaration_ConventionIgnored()
        {
            var convention = ConfigureImportConstructorParameterConvention<ConstructorParameterImportContractX>();
            var mapped = convention.MapType(typeof(ConstructorParameterImportContractX).GetTypeInfo());
            var pi = mapped.GetConstructors().Single().GetParameters().Single();
            var actual = pi.GetCustomAttributes(true);
            AssertEquivalentAttributes(new object[] { new ImportAttribute(ContractNames.ContractX) }, actual);
        }

        // Tests for creation policy

        static RegistrationBuilder ConfigureCreationPolicyConvention<T>()
        {
            var convention = new RegistrationBuilder();
            convention.ForType<T>().SetCreationPolicy(CreationPolicy.NonShared);
            return convention;
        }

        static readonly IEnumerable<object> CreationPolicyConventionAttributes = new[] { new PartCreationPolicyAttribute(CreationPolicy.NonShared) };

        class NoCreationPolicyDeclared { }

        [TestMethod]
        public void CreationPolicyConvention_NoCreationPolicyDeclared_ConventionApplied()
        {
            var convention = ConfigureCreationPolicyConvention<NoCreationPolicyDeclared>();
            AssertHasAttributesUnderConvention<NoCreationPolicyDeclared>(convention, CreationPolicyConventionAttributes);
        }

        [PartCreationPolicy(CreationPolicy.Shared)]
        class SetCreationPolicy { }

        [TestMethod]
        public void CreationPolicyConvention_CreationPolicyDeclared_ConventionIgnored()
        {
            var convention = ConfigureCreationPolicyConvention<SetCreationPolicy>();
            AssertHasDeclaredAttributesUnderConvention<SetCreationPolicy>(convention);
        }

        [Export]
        class UnrelatedToCreationPolicy { }

        [TestMethod]
        public void CreationPolicyConvention_UnrelatedAttributesDeclared_ConventionApplied()
        {
            var convention = ConfigureCreationPolicyConvention<UnrelatedToCreationPolicy>();
            AssertHasAttributesUnderConvention<UnrelatedToCreationPolicy>(convention,
                CreationPolicyConventionAttributes.Concat(typeof(UnrelatedToCreationPolicy).GetCustomAttributes(true)));
        }

        // Tests for part discoverability

        [PartNotDiscoverable]
        class NotDiscoverablePart { }

        [TestMethod]
        public void AnyConvention_NotDiscoverablePart_ConventionApplied()
        {
            var convention = new RegistrationBuilder();
            convention.ForType<NotDiscoverablePart>().Export();
            AssertHasAttributesUnderConvention<NotDiscoverablePart>(convention, new object[] {
                new PartNotDiscoverableAttribute(),
                new ExportAttribute()
            });
        }

        // Tests for part metadata

        static RegistrationBuilder ConfigurePartMetadataConvention<T>()
        {
            var convention = new RegistrationBuilder();
            convention.ForType<T>().AddMetadata(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN);
            return convention;
        }

        static readonly IEnumerable<object> PartMetadataConventionAttributes = new object[]
        {
            new PartMetadataAttribute(MetadataKeys.MetadataKeyP, MetadataValues.MetadataValueN)
        };

        class NoDeclaredPartMetadata { }

        [TestMethod]
        public void PartMetadataConvention_NoDeclaredMetadata_ConventionApplied()
        {
            var convention = ConfigurePartMetadataConvention<NoDeclaredPartMetadata>();
            AssertHasAttributesUnderConvention<NoDeclaredPartMetadata>(convention, PartMetadataConventionAttributes);
        }

        [PartMetadata(MetadataKeys.MetadataKeyQ, MetadataValues.MetadataValueO)]
        class PartMetadataQO { }

        [TestMethod]
        public void PartMetadataConvention_DeclaredQO_ConventionIgnored()
        {
            var convention = ConfigurePartMetadataConvention<PartMetadataQO>();
            AssertHasDeclaredAttributesUnderConvention<PartMetadataQO>(convention);
        }

        [PartCreationPolicy(CreationPolicy.NonShared), Export]
        class PartMetadataUnrelatedAttributes { }

        [TestMethod]
        public void PartMetadataConvention_UnrelatedDeclaredAttributes_ConventionApplied()
        {
            var convention = ConfigurePartMetadataConvention<PartMetadataUnrelatedAttributes>();
            AssertHasAttributesUnderConvention<PartMetadataUnrelatedAttributes>(convention,
                PartMetadataConventionAttributes.Concat(typeof(PartMetadataUnrelatedAttributes).GetCustomAttributes(true)));
        }

        interface IFoo {}
 
        [Export]
        class ExportInterfacesExportOverride : IFoo{ }
        
        class ExportInterfacesExportConventionApplied : IFoo{ }

        [Export(typeof(IFoo))]
        public class ExportInterfacesExportConvention : IFoo { }


        static RegistrationBuilder ConfigureExportInterfacesConvention<T>()
        {
            var convention = new RegistrationBuilder();
            convention.ForType<T>().ExportInterfaces( (t) => true );
            return convention;
        }
 
        [TestMethod]
        public void ConfigureExportInterfaces_ExportInterfaces_Overridden()
        {
            var convention = ConfigureExportInterfacesConvention<ExportInterfacesExportOverride>();
            AssertHasAttributesUnderConvention<ExportInterfacesExportOverride>(convention,
                typeof(ExportInterfacesExportOverride).GetCustomAttributes(true));
        }

        [TestMethod]
        public void ConfigureExportInterfaces_ExportInterfaces_ConventionApplied()
        {
            var convention = ConfigureExportInterfacesConvention<ExportInterfacesExportConventionApplied>();
            AssertHasAttributesUnderConvention<ExportInterfacesExportConventionApplied>(convention,
                typeof(ExportInterfacesExportConvention).GetCustomAttributes(true));
        }

        // Tests for chained RCs
            
        class ConventionTarget { }

        [TestMethod]
        public void ConventionsInInnerAndOuterRCs_InnerRCTakesPrecendence()
        {
            var innerConvention = new RegistrationBuilder();
            innerConvention.ForType<ConventionTarget>().Export(eb => eb.AsContractName(ContractNames.ContractX));
            var innerType = innerConvention.MapType(typeof(ConventionTarget).GetTypeInfo());

            var outerConvention = new RegistrationBuilder();
            outerConvention.ForType<ConventionTarget>().Export(eb => eb.AsContractName(ContractNames.ContractY));
            var outerType = outerConvention.MapType(innerType/*.GetTypeInfo()*/);

            var export = outerType.GetCustomAttributes(false).OfType<ExportAttribute>().Single();

            Assert.AreEqual(ContractNames.ContractX, export.ContractName);
        }
    }
}
