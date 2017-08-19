//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Microsoft.ComponentModel.Composition.Diagnostics.Tests
{
    public class Biff { }

    public interface IBiffMetadata
    {
        string Name { get; }
    }

    public class MissingMeta
    {
        [Export("biff")]
        public Biff Biff = new Biff();
    }

    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WrongCreationPolicy
    {
        [Export("biff"), ExportMetadata("Name", "BiffBiff")]
        public Biff Biff = new Biff();
    }

    public class WrongTypeIdentity
    {
        [Export("biff"), ExportMetadata("Name", "Baf")]
        public string Biff = "Yep";
    }

    [Export]
    public class Bar
    {
        [Import("biff", RequiredCreationPolicy = CreationPolicy.NonShared)]
        public Lazy<Biff, IBiffMetadata> Biff { get; set; }
    }

    [Export]
    public class Foo
    {
        [Import]
        public Bar Bar { get; set; }
    }
}
