using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class ImportOrderingTests : ContainerTests
    {
        public interface IItem
        {
        }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 1)]
        public class Item1 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 4)]
        public class Item4 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 2)]
        public class Item2 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 3)]
        public class Item3 : IItem { }

        [Export(typeof(IItem))]
        public class ItemWithoutOrder : IItem { }

        [Export]
        public class HasImportedItems
        {
            [ImportMany, OrderByMetadata("Order")]
            public IItem[] OrderedItems { get; set; }

            [ImportMany]
            public IItem[] UnorderedItems { get; set; }
        }

        [TestMethod]
        public void CollectionsImportedWithAnOrderingAttributeComeInOrder()
        {
            var container = CreateContainer(typeof(HasImportedItems), typeof(Item1), typeof(Item4), typeof(Item2), typeof(Item3));

            var hasImportedItems = container.GetExport<HasImportedItems>();

            var ordered = hasImportedItems.UnorderedItems.OrderBy(i => i.GetType().Name).ToArray();

            CollectionAssert.AreEqual(ordered, hasImportedItems.OrderedItems);
            CollectionAssert.AreNotEqual(ordered, hasImportedItems.UnorderedItems);
        }

        [TestMethod]
        public void IfAnItemIsMissingMetadataAnInformativeExceptionIsThrown()
        {
            var container = CreateContainer(typeof(HasImportedItems), typeof(Item1), typeof(ItemWithoutOrder));
            var x = AssertX.Throws<LightweightCompositionException>(() => container.GetExport<HasImportedItems>());
            Assert.AreEqual("The metadata 'Order' cannot be used for ordering because it is missing from exports on part(s) 'ItemWithoutOrder'.", x.Message);
        }
    }
}
