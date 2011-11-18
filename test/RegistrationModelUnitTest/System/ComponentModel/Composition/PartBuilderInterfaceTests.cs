namespace System.ComponentModel.Composition
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Registration;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]
    public class PartBuilderInterfaceTests
    {
        public interface IFirst {}
        public interface ISecond {}
        public interface IThird {}
        public interface IFourth {}
        public interface IFifth : IFourth {}

        public class Standard : IFirst, ISecond, IThird, IFifth
        {
        }

        public class Dippy : IFirst, ISecond, IThird, IFifth, IDisposable
        {
            public void Dispose() {}
        }

        public class BareClass {}

        public class Base :  IFirst, ISecond {}

        public class Derived : Base, IThird, IFifth {}

        public class Importer
        {
            [ImportMany] public IEnumerable<IFirst>  First;
            [ImportMany] public IEnumerable<ISecond> Second;
            [ImportMany] public IEnumerable<IThird>  Third;
            [ImportMany] public IEnumerable<IFourth> Fourth;
            [ImportMany] public IEnumerable<IFifth>  Fifth;
            [Import(AllowDefault=true)] public Base         Base;
            [Import(AllowDefault=true)] public Derived      Derived;
            [Import(AllowDefault=true)] public Dippy        Dippy;
            [Import(AllowDefault=true)] public Standard     Standard;
            [Import(AllowDefault=true)] public IDisposable  Disposable;
            [Import(AllowDefault=true)] public BareClass    BareClass;
        }
 
        [TestMethod]
        public void StandardExportInterfacesShouldWork()
        {
            var builder = new RegistrationBuilder();
            builder.ForTypesMatching( (t) => true ).ExportInterfaces( (iface) => iface != typeof(System.IDisposable), (iface, bldr) => bldr.AsContractType((Type)iface) );
            builder.ForTypesMatching((t) => t.GetInterfaces().Where((iface) => iface != typeof(System.IDisposable)).Count() == 0).Export();

            var types = new Type[] { typeof(Standard), typeof(Dippy), typeof(Derived), typeof(BareClass) };
            var catalog = new TypeCatalog(types, builder);

            var cs = catalog.CreateCompositionService();
            
            var importer = new Importer();
            cs.SatisfyImportsOnce(importer);
            
            Assert.IsNotNull(importer.First);
            Assert.IsTrue(importer.First.Count() == 3);
            Assert.IsNotNull(importer.Second);
            Assert.IsTrue(importer.Second.Count() == 3);
            Assert.IsNotNull(importer.Third);
            Assert.IsTrue(importer.Third.Count() == 3);
            Assert.IsNotNull(importer.Fourth);
            Assert.IsTrue(importer.Fourth.Count() == 3);
            Assert.IsNotNull(importer.Fifth);
            Assert.IsTrue(importer.Fifth.Count() == 3);

            Assert.IsNull(importer.Base);
            Assert.IsNull(importer.Derived);
            Assert.IsNull(importer.Dippy);
            Assert.IsNull(importer.Standard);
            Assert.IsNull(importer.Disposable);
            Assert.IsNotNull(importer.BareClass);
        }

        [TestMethod]
        public void StandardExportInterfacesDefaultContractShouldWork()
        {            //Same test as above only using default export builder
            var builder = new RegistrationBuilder();
            builder.ForTypesMatching( (t) => true ).ExportInterfaces( (iface) => iface != typeof(System.IDisposable) );
            builder.ForTypesMatching((t) => t.GetInterfaces().Where((iface) => iface != typeof(System.IDisposable)).Count() == 0).Export();

            var types = new Type[] { typeof(Standard), typeof(Dippy), typeof(Derived), typeof(BareClass) };
            var catalog = new TypeCatalog(types, builder);

            var cs = catalog.CreateCompositionService();
            
            var importer = new Importer();
            cs.SatisfyImportsOnce(importer);
            
            Assert.IsNotNull(importer.First);
            Assert.IsTrue(importer.First.Count() == 3);
            Assert.IsNotNull(importer.Second);
            Assert.IsTrue(importer.Second.Count() == 3);
            Assert.IsNotNull(importer.Third);
            Assert.IsTrue(importer.Third.Count() == 3);
            Assert.IsNotNull(importer.Fourth);
            Assert.IsTrue(importer.Fourth.Count() == 3);
            Assert.IsNotNull(importer.Fifth);
            Assert.IsTrue(importer.Fifth.Count() == 3);

            Assert.IsNull(importer.Base);
            Assert.IsNull(importer.Derived);
            Assert.IsNull(importer.Dippy);
            Assert.IsNull(importer.Standard);
            Assert.IsNull(importer.Disposable);
            Assert.IsNotNull(importer.BareClass);
        }
    }
}
