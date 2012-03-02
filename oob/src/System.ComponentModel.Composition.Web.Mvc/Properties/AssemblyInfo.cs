// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System.ComponentModel.Composition.Web.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("System.ComponentModel.Composition.Web.Mvc")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("System.ComponentModel.Composition.Web.Mvc")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("93f9b9f4-f2fb-4a6e-bcae-37db5193893d")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: PreApplicationStartMethod(typeof(RequestCompositionScopeModule), "Register")]

[assembly: InternalsVisibleTo("System.ComponentModel.Composition.Web.Mvc.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100616470ad6a034af669d130b58deedb7ad8544920d8a21d95bc5bb535ca673d8a49b228c5163f78f34b8df3b015fc2b99ff45b7536830a596f711b8b09f80b48a4bf20883ee5b97f50462d7e0f33440f024dae7d8f7eaf875b747619f1e772131a24dea9d5f80e5d54d95f0704f78fe84ac4b3774ce17eb00a764c295846d43e3")]
