Managed Extensibility Framework
Copyright (c) Microsoft Corporation

/redist contains the solution matching the components of MEF that are
   shipped as part of the .NET Framework. Because these are framework
   assemblies they cannot be "overridden" using typical XCOPY deployment.

/oob contains the solution for components of MEF that ship out-of-band
   via mechanisms such as CodePlex, MSDN downloads or NuGet. These
   are built on top of the redist portion of MEF and can be updated using
   standard deployment mechanisms.
