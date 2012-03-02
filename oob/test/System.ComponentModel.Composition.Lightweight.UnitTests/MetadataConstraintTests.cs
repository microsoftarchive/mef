using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class MetadataConstraintTests : ContainerTests
    {
        [Export, ExportMetadata("SettingName", "TheName")]
        public class SomeSetting { }

        [Export]
        public class SomeSettingUser
        {
            [Import, ImportMetadataConstraint("SettingName", "TheName")]
            public SomeSetting Setting { get; set; }
        }

        [Export]
        public class ManySettingUser
        {
            [ImportMany, ImportMetadataConstraint("SettingName", "TheName")]
            public IEnumerable<SomeSetting> Settings { get; set; }
        }

        [TestMethod]
        public void AnImportMetadataConstraintMatchesMetadataOnTheExport()
        {
            var cc = CreateContainer(typeof(SomeSetting), typeof(SomeSettingUser));
            var ssu = cc.GetExport<SomeSettingUser>();
            Assert.IsNotNull(ssu.Setting);
        }

        [TestMethod]
        public void ImportMetadataConstraintsComposeWithOtherRelationshipTypes()
        {
            var cc = CreateContainer(typeof(SomeSetting), typeof(ManySettingUser));
            var ssu = cc.GetExport<ManySettingUser>();
            Assert.AreEqual(1, ssu.Settings.Count());
        }
    }
}
