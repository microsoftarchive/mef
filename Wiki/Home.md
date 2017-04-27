## Welcome to the MEF Community Site

**Important:** this site hosts source code for both 
**MEF which ships as the part of the .NET framework which ships in box and is not portable 
**the portable revision of MEF available on NuGet. 

|| .NET 4.5 and Windows 8 Store apps can now install the fully-supported, lightweight [Microsoft.Composition package](http://nuget.org/packages/microsoft.composition) from NuGet. ||

### What is it?

The **Managed Extensibility Framework** (MEF) is a composition layer for .NET that improves the flexibility, maintainability and testability of large applications. MEF can be used for third-party plugin extensibility, or it can bring the benefits of a loosely-coupled plugin-like architecture to regular applications.

### Status

MEF is a part of the Microsoft .NET Framework, with types primarily under the  _System.**.Composition.**_ namespaces. There are two versions of MEF 
* System.ComponentModel.Composition.* which has shipped with .NET 4.0 and higher and Silverlight 4.  This provides the standard extension model that has been used in Visual Studio.  The documentation for this version of MEF can be found [here](http://msdn.microsoft.com/en-us/library/system.componentmodel.composition(v=vs.110).aspx)
* System.Compostion.*_  is a lightweight version of MEF, which has been optimized for static composition scenarios and  provides faster compositions.  It is also the only version of MEF that is as a portable class library and can be used on phone, store, desktop and web applications. This version of MEF is available via [NuGet](https://nuget.org/packages/microsoft.composition) and is documentation is available [here](http://msdn.microsoft.com/en-us/library/jj635137(v=vs.110).aspx)
