// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition
{
    [MetadataViewImplementation(typeof(MetadataViewWithImplementation))]
    public interface IMetadataViewWithImplementation
    {
        string String1 { get; }
        string String2 { get; }
    }

    public class MetadataViewWithImplementation: IMetadataViewWithImplementation
    {
        public MetadataViewWithImplementation(IDictionary<string, object> metadata)
        {
            this.String1 = (string)metadata["String1"];
            this.String2 = (string)metadata["String2"];
        }
           
        public string String1 { get; private set; }
        public string String2 { get; private set; }
    }

    [MetadataViewImplementation(typeof(MetadataViewWithImplementationNoInterface))]
    public interface IMetadataViewWithImplementationNoInterface
    {
        string String1 { get; }
        string String2 { get; }
    }
    public class MetadataViewWithImplementationNoInterface
    {
        public MetadataViewWithImplementationNoInterface(IDictionary<string, object> metadata)
        {
            this.String1 = (string)metadata["String1"];
            this.String2 = (string)metadata["String2"];
        }
           
        public string String1 { get; private set; }
        public string String2 { get; private set; }
    }


    [TestClass]
    public class MetadataViewProviderTests
    {

        [TestMethod]
        public void GetMetadataView_InterfaceWithPropertySetter_ShouldThrowNotSupported()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataViewWithPropertySetter>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_InterfaceWithMethod_ShouldThrowNotSupportedException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataViewWithMethod>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_InterfaceWithEvent_ShouldThrowNotSupportedException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataViewWithEvent>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_InterfaceWithIndexer_ShouldThrowNotSupportedException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataViewWithIndexer>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_AbstractClass_ShouldThrowMissingMethodException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<AbstractClassMetadataView>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_AbstractClassWithConstructor_ShouldThrowMemberAccessException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";

            ExceptionAssert.Throws<MemberAccessException>(() =>
            {
                MetadataViewProvider.GetMetadataView<AbstractClassWithConstructorMetadataView>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_IDictionaryAsTMetadataViewTypeArgument_ShouldReturnMetadata()
        {
            var metadata = new Dictionary<string, object>();

            var result = MetadataViewProvider.GetMetadataView<IDictionary<string, object>>(metadata);

            Assert.AreSame(metadata, result);
        }

        [TestMethod]
        public void GetMetadataView_IEnumerableAsTMetadataViewTypeArgument_ShouldReturnMetadata()
        {
            var metadata = new Dictionary<string, object>();

            var result = MetadataViewProvider.GetMetadataView<IEnumerable<KeyValuePair<string, object>>>(metadata);

            Assert.AreSame(metadata, result);
        }


        [TestMethod]
        public void GetMetadataView_DictionaryAsTMetadataViewTypeArgument_ShouldNotThrow()
        {
            var metadata = new Dictionary<string, object>();
            MetadataViewProvider.GetMetadataView<Dictionary<string, object>>(metadata);
        }

        [TestMethod]
        public void GetMetadataView_PrivateInterfaceAsTMetadataViewTypeArgument_ShouldhrowNotSupportedException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["CanActivate"] = true;

            ExceptionAssert.Throws<NotSupportedException>(() =>
                {
                    MetadataViewProvider.GetMetadataView<IActivator>(metadata);
                });
        }

        [TestMethod]
        public void GetMetadataView_DictionaryWithUncastableValueAsMetadataArgument_ShouldThrowCompositionContractMismatchException()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = true;

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataView>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_InterfaceWithTwoPropertiesWithSameNameDifferentTypeAsTMetadataViewArgument_ShouldThrowContractMismatch()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = 10;

            ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataView2>(metadata);
            });            
        }

        [TestMethod]
        public void GetMetadataView_RawMetadata()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = 10;

            var view = MetadataViewProvider.GetMetadataView<RawMetadata>(new Dictionary<string, object>(metadata));

            Assert.IsTrue(view.Count == metadata.Count);
            Assert.IsTrue(view["Value"] == metadata["Value"]);
        }

        [TestMethod]
        public void GetMetadataView_InterfaceInheritance()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = "value";
            metadata["Value2"] = "value2";

            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataView3>(metadata);
            Assert.AreEqual("value", view.Value);
            Assert.AreEqual("value2", view.Value2);
        }
        

        [TestMethod]
        public void GetMetadataView_CachesViewType()
        {
            var metadata1 = new Dictionary<string, object>();
            metadata1["Value"] = "value1";
            var view1 = MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataView>(metadata1);
            Assert.AreEqual("value1", view1.Value);

            var metadata2 = new Dictionary<string, object>();
            metadata2["Value"] = "value2";
            var view2 = MetadataViewProvider.GetMetadataView<ITrans_MetadataTests_MetadataView>(metadata2);
            Assert.AreEqual("value2", view2.Value);

            Assert.AreEqual(view1.GetType(), view2.GetType());
        }

        private interface IActivator
        {
            bool CanActivate
            {
                get;
            }
        }
        public class RawMetadata : Dictionary<string, object>
        {
            public RawMetadata(IDictionary<string, object> dictionary) : base(dictionary) { }
        }

        public abstract class AbstractClassMetadataView
        {
            public abstract object Value { get; }
        }

        public abstract class AbstractClassWithConstructorMetadataView
        {
            public AbstractClassWithConstructorMetadataView(IDictionary<string, object> metadata) {}
            public abstract object Value { get; }
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedInt()
        {
            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedInt>(new Dictionary<string, object>());
            Assert.AreEqual(120, view.MyInt);
        }


        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedIntInTranparentType()
        {
            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedInt>(new Dictionary<string, object>());
            int result = TransparentTestCase.GetMetadataView_IMetadataViewWithDefaultedIntInTranparentType(view);
            Assert.AreEqual(120, result);
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedIntAndInvalidMetadata()
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata = new Dictionary<string, object>();
            metadata.Add("MyInt", 1.2);
            var view1 = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedInt>(metadata);
            Assert.AreEqual(120, view1.MyInt);

            metadata = new Dictionary<string, object>();
            metadata.Add("MyInt", "Hello, World");
            var view2 = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedInt>(metadata);
            Assert.AreEqual(120, view2.MyInt);
        }


        [TestMethod]
        public void GetMetadataView_MetadataViewWithImplementation()
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata = new Dictionary<string, object>();
            metadata.Add("String1", "One");
            metadata.Add("String2", "Two");
            var view1 = MetadataViewProvider.GetMetadataView<IMetadataViewWithImplementation>(metadata);
            Assert.AreEqual("One", view1.String1);
            Assert.AreEqual("Two", view1.String2);
            Assert.AreEqual(view1.GetType(), typeof(MetadataViewWithImplementation));
        }

        [TestMethod]
        public void GetMetadataView_MetadataViewWithImplementationNoInterface()
        {
            var exception = ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                Dictionary<string, object> metadata = new Dictionary<string, object>();
                metadata = new Dictionary<string, object>();
                metadata.Add("String1", "One");
                metadata.Add("String2", "Two");
                var view1 = MetadataViewProvider.GetMetadataView<IMetadataViewWithImplementationNoInterface>(metadata);
            });
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedBool()
        {
            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedBool>(new Dictionary<string, object>());
            Assert.AreEqual(false, view.MyBool);
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedInt64()
        {
            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedInt64>(new Dictionary<string, object>());
            Assert.AreEqual(Int64.MaxValue, view.MyInt64);
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithDefaultedString()
        {
            var view = MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithDefaultedString>(new Dictionary<string, object>());
            Assert.AreEqual("MyString", view.MyString);
        }
        [TestMethod]
        public void GetMetadataView_IMetadataViewWithTypeMismatchDefaultValue()
        {
            var exception = ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithTypeMismatchDefaultValue>(new Dictionary<string, object>());
            });

            Assert.IsInstanceOfType(exception.InnerException, typeof(TargetInvocationException));
        }

        [TestMethod]
        public void GetMetadataView_IMetadataViewWithTypeMismatchOnUnbox()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = (short)9999;

            var exception = ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_MetadataViewWithTypeMismatchDefaultValue>(new Dictionary<string, object>());
            });

            Assert.IsInstanceOfType(exception.InnerException, typeof(TargetInvocationException));
        }

        [TestMethod]
        public void TestMetadataIntConversion()
        {
            var metadata = new Dictionary<string, object>();
            metadata["Value"] = (Int64)45;

            var exception = ExceptionAssert.Throws<CompositionContractMismatchException>(() =>
            {
                MetadataViewProvider.GetMetadataView<ITrans_HasInt64>(metadata);
            });
        }

    }
}
