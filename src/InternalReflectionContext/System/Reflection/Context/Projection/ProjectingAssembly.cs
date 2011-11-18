// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Globalization;
using System.Reflection.Context.Delegation;
using System.Diagnostics.Contracts;

namespace System.Reflection.Context.Projection
{
    // Recursively 'projects' any assemblies, modules, types and members returned by a given assembly
    internal class ProjectingAssembly : DelegatingAssembly, IProjectable
    {
        private readonly Projector _projector;

        public ProjectingAssembly(Assembly assembly, Projector projector)
            : base(assembly)
        {
            Contract.Requires(null != projector);

            _projector = projector;
        }

        public Projector Projector
        {
            get { return _projector; }
        }

        #region Assembly overrides
        public override Module ManifestModule
        {
            get { return _projector.ProjectModule(base.ManifestModule); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            attributeType = _projector.Unproject(attributeType);

            return base.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _projector.Project(base.GetCustomAttributesData(), _projector.ProjectCustomAttributeData);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            attributeType = _projector.Unproject(attributeType);

            return base.IsDefined(attributeType, inherit);
        }

        public override MethodInfo EntryPoint
        {
            get { return _projector.ProjectMethod(base.EntryPoint); }
        }

        public override Type[] GetExportedTypes()
        {
            return _projector.Project(base.GetExportedTypes(), _projector.ProjectType);
        }

        public override Module[] GetLoadedModules(bool getResourceModules)
        {
            return _projector.Project(base.GetLoadedModules(getResourceModules), _projector.ProjectModule);
        }

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            return _projector.ProjectManifestResource(base.GetManifestResourceInfo(resourceName));
        }

        public override Module GetModule(string name)
        {
            return _projector.ProjectModule(base.GetModule(name));
        }

        public override Module[] GetModules(bool getResourceModules)
        {
            return _projector.Project(base.GetModules(getResourceModules), _projector.ProjectModule);
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            return _projector.ProjectAssembly(base.GetSatelliteAssembly(culture));
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            return _projector.ProjectAssembly(base.GetSatelliteAssembly(culture, version));
        }

        public override Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            return _projector.ProjectType(base.GetType(name, throwOnError, ignoreCase));
        }

        public override Type[] GetTypes()
        {
            return _projector.Project(base.GetTypes(), _projector.ProjectType);
        }

        public override Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            return _projector.ProjectModule(base.LoadModule(moduleName, rawModule, rawSymbolStore));
        }
        #endregion

        #region object overrides
        public override bool Equals(object o)
        {
            ProjectingAssembly other = o as ProjectingAssembly;
            return other != null &&
                   Projector == other.Projector &&
                   UnderlyingAssembly == other.UnderlyingAssembly;
        }

        public override int GetHashCode()
        {
            return Projector.GetHashCode() ^ UnderlyingAssembly.GetHashCode();
        }
        #endregion
    }
}
