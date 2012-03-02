// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
#if MEF_FEATURE_REGISTRATION
using System.ComponentModel.Composition.Registration;
#endif //MEF_FEATURE_REGISTRATION
namespace System.ComponentModel.Composition
{
    public interface IContract {}
    public class ContractImpl : IContract {}
    public class MyEmptyClass
    {
        public ExportFactory<IContract> MyFactoryProperty { get; set; }
        public IContract MyProperty { get; set; }
    }

    public class MyEmptyClassWithFactoryConstructor : MyEmptyClass
    {
        [ImportingConstructor]
        public MyEmptyClassWithFactoryConstructor([Import(RequiredCreationPolicy=CreationPolicy.NewScope)]ExportFactory<IContract> myFactoryProperty) {  this.MyFactoryProperty = myFactoryProperty; }
    }
    public class MyEmptyClassWithStandardConstructor : MyEmptyClass
    {
        [ImportingConstructor]
        public MyEmptyClassWithStandardConstructor([Import(RequiredCreationPolicy=CreationPolicy.NewScope)]IContract myProperty) {  this.MyProperty = myProperty; }
    }

    internal static class Helpers
    {
        public static IEnumerable<Type> GetEnumerableOfTypes(params Type[] types)
        {
            return types;
        }
    }

    [TestClass]
    public class ComposablePartDefinitionTests
    {
        [TestMethod]
        public void Constructor1_ShouldNotThrow()
        {
            PartDefinitionFactory.Create();
        }

        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var definition = PartDefinitionFactory.Create();

            EnumerableAssert.IsEmpty(definition.Metadata);
        }

        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var definition = PartDefinitionFactory.Create();

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                definition.Metadata["Value"] = "Value";
            });
        }

#if MEF_FEATURE_REGISTRATION
        [TestMethod]
        public void CreatePart_ImportHasNewScope_NewscopeOnExportFactoryShouldSucceed()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<ContractImpl>().Export<IContract>();
            ctx.ForType<MyEmptyClass>().Export<MyEmptyClass>();
            ctx.ForType<MyEmptyClass>().ImportProperty( (import) => import.MyFactoryProperty, (c) => c.RequiredCreationPolicy(CreationPolicy.NewScope) );
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(MyEmptyClass), typeof(ContractImpl)), ctx); 

            foreach (var p in catalog.Parts)
            {
                var md = p.ImportDefinitions.Count();
            }
        }

        [TestMethod]
        public void CreatePart_ImportHasNewScope_NewscopeOnStandardImportShouldThrowComposablePartException()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<ContractImpl>().Export<IContract>();
            ctx.ForType<MyEmptyClass>().Export<MyEmptyClass>();
            ctx.ForType<MyEmptyClass>().ImportProperty( (import) => import.MyProperty, (c) => c.RequiredCreationPolicy(CreationPolicy.NewScope) );
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(MyEmptyClass), typeof(ContractImpl)), ctx); 
        
            ExceptionAssert.Throws<ComposablePartException>(() =>
            {
                foreach (var p in catalog.Parts)
                {
                    var md = p.ImportDefinitions.Count();
                }
            });
        }

        [TestMethod]
        public void CreatePart_ImportHasNewScope_NewscopeOnConstructorExportFactoryImportShouldSucceed()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<ContractImpl>().Export<IContract>();
            ctx.ForType<MyEmptyClassWithFactoryConstructor>().Export<MyEmptyClassWithFactoryConstructor>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(MyEmptyClassWithFactoryConstructor), typeof(ContractImpl)), ctx); 

            foreach (var p in catalog.Parts)
            {
                var md = p.ImportDefinitions.Count();
            }
        }

        [TestMethod]
        public void CreatePart_ImportHasNewScope_NewscopeOnConstructorStandardImportShouldThrowComposablePartException()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<ContractImpl>().Export<IContract>();
            ctx.ForType<MyEmptyClassWithStandardConstructor>().Export<MyEmptyClassWithStandardConstructor>();
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(MyEmptyClassWithStandardConstructor), typeof(ContractImpl)), ctx); 

            ExceptionAssert.Throws<ComposablePartException>(() =>
            {
                foreach (var p in catalog.Parts)
                {
                    var md = p.ImportDefinitions.Count();
                }
            });
        }

        [TestMethod]
        public void CreatePart_NewScopeOnExportThrowComposablePartException()
        {
            var ctx = new RegistrationBuilder();
            ctx.ForType<ContractImpl>().Export<IContract>();
            ctx.ForType<MyEmptyClass>().Export<MyEmptyClass>().SetCreationPolicy(CreationPolicy.NewScope);
            var catalog = new TypeCatalog(Helpers.GetEnumerableOfTypes(typeof(MyEmptyClass), typeof(ContractImpl)), ctx); 
        
            ExceptionAssert.Throws<ComposablePartException>(() =>
            {
                foreach (var p in catalog.Parts)
                {
                    var md = p.Metadata;
                }
            });
        }
#endif //MEF_FEATURE_REGISTRATION
    }
}

