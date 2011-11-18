// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Delegation
{
    internal class DelegatingAssembly : Assembly
    {
        private readonly Assembly _assembly;

        // We cannot override ModuleResolve and Permissionset because they are critical.
        // Users will get NotImplementedException when calling these two APIs.

        public DelegatingAssembly(Assembly assembly)
        {
            Contract.Requires(null != assembly);

            _assembly = assembly;
        }

        public override string Location
        {
            get { return _assembly.Location; }
        }

        public override Module ManifestModule
        {
            get { return _assembly.ManifestModule; }
        }

        public override bool ReflectionOnly
        {
            get { return _assembly.ReflectionOnly; }
        }

        public Assembly UnderlyingAssembly
        {
            get { return _assembly; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _assembly.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _assembly.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _assembly.GetCustomAttributesData();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _assembly.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _assembly.ToString();
        }

        public override SecurityRuleSet SecurityRuleSet
        {
            get { return _assembly.SecurityRuleSet; }
        }

        public override string CodeBase
        {
            get { return _assembly.CodeBase; }
        }

        public override object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            return _assembly.CreateInstance(typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);
        }

        public override MethodInfo EntryPoint
        {
            get { return _assembly.EntryPoint; }
        }

        public override string EscapedCodeBase
        {
            get { return _assembly.EscapedCodeBase; }
        }

        public override Evidence Evidence
        {
            get { return _assembly.Evidence; }
        }

        public override string FullName
        {
            get { return _assembly.FullName; }
        }

        public override Type[] GetExportedTypes()
        {
            return _assembly.GetExportedTypes();
        }

        public override FileStream GetFile(string name)
        {
            return _assembly.GetFile(name);
        }

        public override FileStream[] GetFiles()
        {
            return _assembly.GetFiles();
        }

        public override FileStream[] GetFiles(bool getResourceModules)
        {
            return _assembly.GetFiles(getResourceModules);
        }

        public override Module[] GetLoadedModules(bool getResourceModules)
        {
            return _assembly.GetLoadedModules(getResourceModules);
        }

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            return _assembly.GetManifestResourceInfo(resourceName);
        }

        public override string[] GetManifestResourceNames()
        {
            return _assembly.GetManifestResourceNames();
        }

        public override Stream GetManifestResourceStream(string name)
        {
            return _assembly.GetManifestResourceStream(name);
        }

        public override Stream GetManifestResourceStream(Type type, string name)
        {
            return _assembly.GetManifestResourceStream(type, name);
        }

        public override Module GetModule(string name)
        {
            return _assembly.GetModule(name);
        }

        public override Module[] GetModules(bool getResourceModules)
        {
            return _assembly.GetModules(getResourceModules);
        }

        public override AssemblyName GetName()
        {
            return _assembly.GetName();
        }

        public override AssemblyName GetName(bool copiedName)
        {
            return _assembly.GetName(copiedName);
        }

        public override AssemblyName[] GetReferencedAssemblies()
        {
            return _assembly.GetReferencedAssemblies();
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            return _assembly.GetSatelliteAssembly(culture);
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            return _assembly.GetSatelliteAssembly(culture, version);
        }

        public override Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            return _assembly.GetType(name, throwOnError, ignoreCase);
        }

        public override Type[] GetTypes()
        {
            return _assembly.GetTypes();
        }

        public override bool GlobalAssemblyCache
        {
            get { return _assembly.GlobalAssemblyCache; }
        }

        public override long HostContext
        {
            get { return _assembly.HostContext; }
        }

        public override string ImageRuntimeVersion
        {
            get { return _assembly.ImageRuntimeVersion; }
        }

        public override bool IsDynamic
        {
            get { return _assembly.IsDynamic; }
        }

        public override Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            return _assembly.LoadModule(moduleName, rawModule, rawSymbolStore);
        }
    }
}
