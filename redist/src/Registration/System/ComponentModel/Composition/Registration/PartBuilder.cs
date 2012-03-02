using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Registration
{
    public class PartBuilder
    {
        private readonly static List<Attribute> ImportingConstructorList = new List<Attribute>() { new ImportingConstructorAttribute() };
        private static readonly Type ExportAttributeType = typeof(ExportAttribute);
        private List<ExportBuilder> _typeExportBuilders;
        private List<ImportBuilder> _constructorImportBuilders;
        private bool _setCreationPolicy = false;
        private CreationPolicy _creationPolicy;

        // Metadata selection
        private List<Tuple<string, object>> _metadataItems;
        private List<Tuple< string, Func<Type, object>>> _metadataItemFuncs;

        // Constructor selector / configuration
        private Func<ConstructorInfo[], ConstructorInfo> _constructorFilter;
        private Action<ParameterInfo, ImportBuilder> _configureConstuctorImports;

        //Property Import/Export selection and configuration
        private List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, ExportBuilder>, Type>> _propertyExports;
        private List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, ImportBuilder>, Type>> _propertyImports;
        private List<Tuple<Predicate<Type>, Action<Type, ExportBuilder>>> _interfaceExports;

        internal Predicate<Type> SelectType { get; private set; }

        internal PartBuilder(Predicate<Type> selectType)
        {
            this.SelectType = selectType;
            this._setCreationPolicy = false;
            this._creationPolicy = CreationPolicy.Any;
            this._typeExportBuilders = new List<ExportBuilder>();
            this._constructorImportBuilders = new List<ImportBuilder>();
            this._propertyExports = new List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, ExportBuilder>, Type>>();
            this._propertyImports = new List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, ImportBuilder>, Type>>();
            this._interfaceExports = new List<Tuple<Predicate<Type>, Action<Type, ExportBuilder>>>();
        }

        public PartBuilder Export()
        {
            return Export(null);
        }

        public PartBuilder Export(Action<ExportBuilder> exportConfiguration)
        {
            var exportBuilder = new ExportBuilder();
            if (exportConfiguration != null)
            {
                exportConfiguration(exportBuilder);
            }

            this._typeExportBuilders.Add(exportBuilder);
            return this;
        }

        public PartBuilder Export<T>()
        {
            return Export<T>(null);
        }

        public PartBuilder Export<T>(Action<ExportBuilder> exportConfiguration)
        {
            var exportBuilder = new ExportBuilder().AsContractType<T>();
            if (exportConfiguration != null)
            {
                exportConfiguration(exportBuilder);
            }
            this._typeExportBuilders.Add(exportBuilder);
            return this;
        }

        // Choose a constructor from all of the available constructors, then configure them
        public PartBuilder SelectConstructor(Func<ConstructorInfo[], ConstructorInfo> constructorFilter)
        {
            return SelectConstructor(constructorFilter, null);
        }

        public PartBuilder SelectConstructor(
            Func<ConstructorInfo[], ConstructorInfo> constructorFilter, 
            Action<ParameterInfo, ImportBuilder> importConfiguration)
        {
            this._constructorFilter = constructorFilter;
            this._configureConstuctorImports = importConfiguration;
            return this;
        }

        // Choose an interface to export then configure it
        public PartBuilder ExportInterfaces(Predicate<Type> interfaceFilter)
        {
            return ExportInterfaces(interfaceFilter, null);
        }

        public PartBuilder ExportInterfaces()
        {
            return ExportInterfaces(t => true, null);
        }

        public PartBuilder ExportInterfaces(
            Predicate<Type> interfaceFilter,
            Action<Type, ExportBuilder> exportConfiguration)
        {
            Requires.NotNull(interfaceFilter, "interfaceFilter");
            this._interfaceExports.Add(Tuple.Create(interfaceFilter, exportConfiguration));
            return this;
        }

        // Choose a property to export then configure it
        public PartBuilder ExportProperties(Predicate<PropertyInfo> propertyFilter)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            return ExportProperties(propertyFilter, null);
        }

        public PartBuilder ExportProperties(
            Predicate<PropertyInfo> propertyFilter, 
            Action<PropertyInfo, ExportBuilder> exportConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            this._propertyExports.Add(Tuple.Create(propertyFilter, exportConfiguration, default(Type)));
            return this;
        }

        // Choose a property to export then configure it
        public PartBuilder ExportProperties<T>(Predicate<PropertyInfo> propertyFilter)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            return ExportProperties<T>(propertyFilter, null);
        }

        public PartBuilder ExportProperties<T>(
            Predicate<PropertyInfo> propertyFilter, 
            Action<PropertyInfo, ExportBuilder> exportConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            this._propertyExports.Add(Tuple.Create(propertyFilter, exportConfiguration, typeof(T)));
            return this;
        }

        // Choose a property to export then configure it
        public PartBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            return ImportProperties(propertyFilter, null);
        }

        public PartBuilder ImportProperties(
            Predicate<PropertyInfo> propertyFilter, 
            Action<PropertyInfo, ImportBuilder> importConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            this._propertyImports.Add(Tuple.Create(propertyFilter, importConfiguration, default(Type)));
            return this;
        }

        // Choose a property to export then configure it
        public PartBuilder ImportProperties<T>(Predicate<PropertyInfo> propertyFilter)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            return ImportProperties<T>(propertyFilter, null);
        }

        public PartBuilder ImportProperties<T>(
            Predicate<PropertyInfo> propertyFilter, 
            Action<PropertyInfo, ImportBuilder> importConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            this._propertyImports.Add(Tuple.Create(propertyFilter, importConfiguration, typeof(T)));
            return this;
        }

        public PartBuilder SetCreationPolicy(CreationPolicy creationPolicy)
        {
            this._setCreationPolicy = true;
            this._creationPolicy = creationPolicy;
            return this;
        }

        public PartBuilder AddMetadata(string name, object value)
        {
            if(this._metadataItems == null)
            {
                this._metadataItems = new List<Tuple<string, object>>();
            }
            this._metadataItems.Add(Tuple.Create(name, value));
            return this;
        }

        public PartBuilder AddMetadata(string name, Func<Type, object> itemFunc)
        {
            if(this._metadataItemFuncs == null)
            {
                this._metadataItemFuncs = new List<Tuple<string, Func<Type, object>>>();
            }
            this._metadataItemFuncs.Add(Tuple.Create(name, itemFunc));
            return this;
        }

        static bool MemberHasExportMetadata(MemberInfo member)
        {
            foreach (var attr in member.GetAttributes<Attribute>())
            {
                var provider = attr as ExportMetadataAttribute;
                if (provider != null)
                {
                    return true;
                }
                else
                {
                    Type attrType = attr.GetType();
                    // Perf optimization, relies on short circuit evaluation, often a property attribute is an ExportAttribute
                    if (attrType != PartBuilder.ExportAttributeType && attrType.IsAttributeDefined<MetadataAttributeAttribute>(true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal IEnumerable<Attribute> BuildTypeAttributes(Type type)
        {
            var attributes = new List<Attribute>();

            if(this._typeExportBuilders != null)
            {
                bool isConfigured = type.GetFirstAttribute<ExportAttribute>() != null || MemberHasExportMetadata(type);
                if(isConfigured)
                {
                    CompositionTrace.Registration_TypeExportConventionOverridden(type);
                }
                else
                {
                    foreach (var export in this._typeExportBuilders)
                    {
                        export.BuildAttributes(type, ref attributes);
                    }
                }
            }

            if (this._setCreationPolicy)
            {
                // Check if there is already a PartCreationPolicyAttribute
                // If found Trace a warning and do not add the registered part creationpolicy
                // otherwise add new one
                bool isConfigured = type.GetFirstAttribute<PartCreationPolicyAttribute>() != null;
                if(isConfigured)
                {
                    CompositionTrace.Registration_PartCreationConventionOverridden(type);
                }
                else
                {
                    attributes.Add(new PartCreationPolicyAttribute(this._creationPolicy));
                }
            }

            //Add metadata attributes from direct specification
            if (this._metadataItems != null)
            {
                bool isConfigured = type.GetFirstAttribute<PartMetadataAttribute>() != null;
                if(isConfigured)
                {
                    CompositionTrace.Registration_PartMetadataConventionOverridden(type);
                }
                else
                {
                    foreach (var item in this._metadataItems)
                    {
                        attributes.Add(new PartMetadataAttribute(item.Item1, item.Item2));
                    }
                }
            }

            //Add metadata attributes from func specification
            if (this._metadataItemFuncs != null)
            {
                bool isConfigured = type.GetFirstAttribute<PartMetadataAttribute>() != null;
                if(isConfigured)
                {
                    CompositionTrace.Registration_PartMetadataConventionOverridden(type);
                }
                else
                {
                    foreach (var item in this._metadataItemFuncs)
                    {
                        var name = item.Item1;
                        var value = (item.Item2 != null) ? item.Item2(type) : null;
                        attributes.Add(new PartMetadataAttribute(name, value));
                    }
                }
            }

            if(this._interfaceExports.Any())
            {
                if(this._typeExportBuilders != null)
                {
                    bool isConfigured = type.GetFirstAttribute<ExportAttribute>() != null || MemberHasExportMetadata(type);
                    if(isConfigured)
                    {
                        CompositionTrace.Registration_TypeExportConventionOverridden(type);
                    }
                    else
                    {
                        foreach (var iface in type.GetInterfaces())
                        {
                            var underlyingType = ((Type)iface).UnderlyingSystemType;

                            if(underlyingType == typeof(IDisposable) || underlyingType == typeof(IPartImportsSatisfiedNotification) )
                            {
                                continue;
                            }

                            // Run through the export specifications see if any match
                            foreach (var exportSpecification in this._interfaceExports)
                            {
                                if (exportSpecification.Item1 != null && exportSpecification.Item1(underlyingType))
                                {
                                    ExportBuilder exportBuilder = new ExportBuilder();
                                    exportBuilder.AsContractType((Type)iface);
                                    if (exportSpecification.Item2 != null)
                                    {
                                        exportSpecification.Item2(iface, exportBuilder);
                                    }
                                    exportBuilder.BuildAttributes(iface, ref attributes);
                                }
                            }
                        }
                    }
                    
                }
            }            
            return attributes;
        }

        
        internal   bool BuildConstructorAttributes(Type type, ref List<Tuple<object, List<Attribute>>> configuredMembers)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            
            // First see if any of these constructors have the ImportingConstructorAttribute if so then we are already done
            foreach(var ci in constructors)
            {
                // We have a constructor configuration we must log a warning then not bother with ConstructorAttributes
                object[] attributes = ci.GetCustomAttributes(typeof(ImportingConstructorAttribute), false);
                if(attributes.Length != 0)
                {
                    CompositionTrace.Registration_ConstructorConventionOverridden(type);
                    return true;
                }
            }

            if (this._constructorFilter != null)
            {
                ConstructorInfo constructorInfo = this._constructorFilter(constructors);
                if(constructorInfo != null)
                {
                    ConfigureConstructorAttributes(constructorInfo, ref configuredMembers, this._configureConstuctorImports);
                }
                return true;
            }
            else if (this._configureConstuctorImports != null)
            {
                bool configured = false;
                foreach(var constructorInfo in FindLongestConstructors(constructors))
                {
                    ConfigureConstructorAttributes(constructorInfo, ref configuredMembers, this._configureConstuctorImports);
                    configured = true;
                }
                return configured;
            }
            return false;
        }

        internal static void BuildDefaultConstructorAttributes(Type type, ref List<Tuple<object, List<Attribute>>> configuredMembers)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach(var constructorInfo in FindLongestConstructors(constructors))
            {
                ConfigureConstructorAttributes(constructorInfo, ref configuredMembers, null);
            }
        }

        private static void ConfigureConstructorAttributes(ConstructorInfo constructorInfo, ref List<Tuple<object, List<Attribute>>> configuredMembers, Action<ParameterInfo, ImportBuilder> configureConstuctorImports)
        {
            if(configuredMembers == null)
            {
                configuredMembers = new List<Tuple<object, List<Attribute>>>();
            }

            // Make its attribute
            configuredMembers.Add(Tuple.Create((object)constructorInfo, ImportingConstructorList));

            //Okay we have the constructor now we can configure the ImportBuilders
            var parameterInfos = constructorInfo.GetParameters();
            foreach (var pi in parameterInfos)
            {
                bool isConfigured = pi.GetFirstAttribute<ImportAttribute>() != null || pi.GetFirstAttribute<ImportManyAttribute>() != null;
                if(isConfigured)
                {
                    CompositionTrace.Registration_ParameterImportConventionOverridden(pi, constructorInfo);
                }
                else
                {
                    var importBuilder = new ImportBuilder();

                    // Let the developer alter them if they specified to do so
                    if (configureConstuctorImports != null)
                    {
                        configureConstuctorImports(pi, importBuilder);
                    }

                    // Generate the attributes
                    List<Attribute> attributes = null;
                    importBuilder.BuildAttributes(pi.ParameterType, ref attributes);
                    configuredMembers.Add(Tuple.Create((object)pi, attributes));
                }
            }
        }

        internal void BuildPropertyAttributes(Type type, ref List<Tuple<object, List<Attribute>>> configuredMembers)
        {
            if(this._propertyImports.Any() || this._propertyExports.Any())
            {
                foreach (var pi in type.GetProperties())
                {
                    List<Attribute> attributes = null;
                    var declaredPi = pi.DeclaringType.UnderlyingSystemType.GetProperty(pi.Name, pi.PropertyType);
                    int importsBuilt = 0;
                    bool checkedIfConfigured = false;
                    bool isConfigured = false;

                    // Run through the import specifications see if any match
                    foreach(var importSpecification in this._propertyImports)
                    {
                        if(importSpecification.Item1 != null && importSpecification.Item1(declaredPi))
                        {
                            var importBuilder = new ImportBuilder();
    
                            if (importSpecification.Item3 != null)
                            {
                                importBuilder.AsContractType(importSpecification.Item3);
                            }

                            if(importSpecification.Item2 != null)
                            {
                                importSpecification.Item2(declaredPi, importBuilder);
                            }

                            if(!checkedIfConfigured)
                            {
                                isConfigured = pi.GetFirstAttribute<ImportAttribute>() != null || pi.GetFirstAttribute<ImportManyAttribute>() != null;
                                checkedIfConfigured = true;
                            }

                            if(isConfigured)
                            {
                                CompositionTrace.Registration_MemberImportConventionOverridden(type, pi);
                                break;
                            }
                            else
                            {
                                importBuilder.BuildAttributes(declaredPi.PropertyType, ref attributes);
                                ++importsBuilt;
                            }
                        }
                        if(importsBuilt > 1)
                        {
                            CompositionTrace.Registration_MemberImportConventionMatchedTwice(type, pi);
                        }
                    }
    
                    checkedIfConfigured = false;
                    isConfigured = false;

                    // Run through the export specifications see if any match
                    foreach(var exportSpecification in this._propertyExports)
                    {
                        if (exportSpecification.Item1 != null && exportSpecification.Item1(declaredPi))
                        {
                            var exportBuilder = new ExportBuilder();

                            if (exportSpecification.Item3 != null)
                            {
                                exportBuilder.AsContractType(exportSpecification.Item3);
                            }

                            if(exportSpecification.Item2 != null)
                            {
                                exportSpecification.Item2(declaredPi, exportBuilder);
                            }

                            if(!checkedIfConfigured)
                            {
                                isConfigured = pi.GetFirstAttribute<ExportAttribute>() != null || MemberHasExportMetadata(pi);
                                checkedIfConfigured = true;
                            }

                            if(isConfigured)
                            {
                                CompositionTrace.Registration_MemberExportConventionOverridden(type, pi);
                                break;
                            }
                            else
                            {
                                exportBuilder.BuildAttributes(declaredPi.PropertyType, ref attributes);
                            }
                        }
                    }
    
                    if(attributes != null)
                    {
                        if(configuredMembers == null)
                        {
                            configuredMembers = new List<Tuple<object, List<Attribute>>>();
                        }
    
                        configuredMembers.Add(Tuple.Create((object)declaredPi, attributes));
                    }
                }
            }
            return;
        }
        static IEnumerable<ConstructorInfo>FindLongestConstructors(ConstructorInfo[] constructors)
        {
            ConstructorInfo longestConstructor = null;
            int argumentsCount = 0;
            int constructorsFound = 0;

            foreach(var candidateConstructor in constructors)
            {
                int length = candidateConstructor.GetParameters().Length;
                if(length != 0)
                {
                    if(length > argumentsCount)
                    {
                        longestConstructor = candidateConstructor;
                        argumentsCount = length;
                        constructorsFound = 1;
                    }
                    else if(length == argumentsCount)
                    {
                        ++constructorsFound;
                    }
                }
            }
            if(constructorsFound > 1)
            {
                foreach(var candidateConstructor in constructors)
                {
                    int length = candidateConstructor.GetParameters().Length;
                    if(length == argumentsCount)
                    {
                        yield return candidateConstructor;
                    }
                }
            }
            else if(constructorsFound == 1)
            {
                yield return longestConstructor;
            }
            yield break;
        }
    }
}
