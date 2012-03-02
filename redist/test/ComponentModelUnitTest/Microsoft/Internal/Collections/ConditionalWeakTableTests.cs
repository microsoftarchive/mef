// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Internal.Collections
{
    [TestClass]
    public class ConditionalWeakTableTests
    {
        [TestMethod]
        public void Add_KeyShouldBeCollected()
        {
            var obj = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, new object());

            var wr = new WeakReference(obj);

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull(wr.Target, "Key should be collected now!");

            GC.KeepAlive(cwt);
        }

        [TestMethod]
        public void Add_KeyHeld_ValueShouldNotBeCollected()
        {
            var obj = new object();
            var str = new StringBuilder();
            var cwt = new ConditionalWeakTable<object, StringBuilder>();

            var wrKey = new WeakReference(obj);
            var wrValue = new WeakReference(str);

            cwt.Add(obj, str);

            str = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Should still have both references
            Assert.IsNotNull(wrKey.Target, "Key should NOT be collected yet!");
            Assert.IsNotNull(wrValue.Target, "Value should NOT be collected yet!");

            GC.KeepAlive(obj);
            GC.KeepAlive(cwt);
        }

        [TestMethod]
        public void Add_KeyCollected_ValueShouldBeCollected()
        {
            var obj = new object();
            var str = new StringBuilder();
            var cwt = new ConditionalWeakTable<object, StringBuilder>();

            cwt.Add(obj, str);

            var wrKey = new WeakReference(obj);
            var wrValue = new WeakReference(str);
            str = null;
            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull(wrKey.Target, "Key should be collected now!");
            Assert.IsNull(wrValue.Target, "Value should be collected now!");

            GC.KeepAlive(cwt);
        }

        [TestMethod]
        public void Remove_ValidKey_ShouldReturnTrue()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            Assert.IsTrue(cwt.Remove(obj));
        }

        [TestMethod]
        public void Remove_InvalidKey_ShouldReturnTrue()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            Assert.IsFalse(cwt.Remove(obj2));
        }

        [TestMethod]
        public void TryGetValue_ValidKey_ShouldReturnTrueAndValue()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            object obj3;
            Assert.IsTrue(cwt.TryGetValue(obj, out obj3), "Should find a value with the key!");
            Assert.AreEqual(obj2, obj3);
        }

        [TestMethod]
        public void TryGetValue_InvalidKey_ShouldReturnFalseAndNull()
        {
            var obj = new object();
            var obj2 = new object();
            var cwt = new ConditionalWeakTable<object, object>();

            cwt.Add(obj, obj2);

            object obj3;
            Assert.IsFalse(cwt.TryGetValue(obj2, out obj3), "Should NOT find a value with the key!");
            Assert.IsNull(obj3); 
        }
    }
}
