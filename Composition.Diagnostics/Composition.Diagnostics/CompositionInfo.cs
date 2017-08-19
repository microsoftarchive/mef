//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.IO;
using System.ComponentModel.Composition.ReflectionModel;

namespace Microsoft.ComponentModel.Composition.Diagnostics
{
    /// <summary>
    /// Analysis of a catalog of parts within a composition host.
    /// </summary>
    public class CompositionInfo
    {
        Lazy<IEnumerable<CompositionContractInfo>> _contracts;

        /// <summary>
        /// The catalog of parts under analysis.
        /// </summary>
        public ComposablePartCatalog Catalog { get; private set; }
        
        /// <summary>
        /// The host environment in which the parts provide exports and have
        /// their imports statisfied.
        /// </summary>
        public ExportProvider Host { get; private set; }

        public IEnumerable<Type> Types { get; private set; }

        private Dictionary<ComposablePartDefinition, Type> _partToType;
        private Dictionary<Type, ComposablePartDefinition> _typeToPart;

        /// <summary>
        /// Analysis of all parts in the catalog.
        /// </summary>
        public IEnumerable<PartDefinitionInfo> PartDefinitions { get; private set; }

        /// <summary>
        /// Analysis of all contracts imported and/or exported by the parts
        /// in the catalog.
        /// </summary>
        public IEnumerable<CompositionContractInfo> Contracts
        {
            get
            {
                return _contracts.Value; 
            }
        }

        public CompositionInfo(ComposablePartCatalog catalog, ExportProvider host)
            : this(catalog, host, GetTypesFromCatalog(catalog))
        {

        }

        /// <summary>
        /// Create a new CompositionInfo.
        /// </summary>
        /// <param name="catalog">The catalog to analyze.</param>
        /// <param name="host">The host context in which to perform the analysis.</param>
        /// <param name="types">The types to analyze</param>
        /// <remarks>Most commonly, host will be an instance of CompositionContainer.</remarks>
        public CompositionInfo(ComposablePartCatalog catalog, ExportProvider host, IEnumerable<Type> types)
        {
            Catalog = catalog;
            Host = host;
            //  Convert to a list now so that it won't keep re-calculating the list of types
            Types = types.ToList();

            _partToType = new Dictionary<ComposablePartDefinition, Type>();
            _typeToPart = catalog.Parts.ToDictionary(p => GetTypeForPart(p), p => p);          


            _contracts = new Lazy<IEnumerable<CompositionContractInfo>>(() => AnalyzeContracts());

            // Two step because relationships can cycle

            var partInfos = catalog
                .Parts.Union(types.Select(type => GetPartForType(type)).Where(p => p != null))
                .Select(pd => new PartDefinitionInfo(pd))
                .ToArray();

            foreach (var pdi in partInfos)
            {
                pdi.ImportDefinitions = pdi.PartDefinition.ImportDefinitions
                    .Select(id => AnalyzeImportDefinition(host, partInfos, id));
            }

            PartDefinitions = partInfos;
            //PartDefinitions = Enumerable.Empty<PartDefinitionInfo>();
        }

        private static IEnumerable<Type> GetTypesFromCatalog(ComposablePartCatalog catalog)
        {
            if (catalog is AggregateCatalog)
            {
                return ((AggregateCatalog)catalog).Catalogs.SelectMany(c => GetTypesFromCatalog(c));
            }
            else if (catalog is TypeCatalog ||
                IsDeploymentCatalog(catalog))
            {
                return catalog.Parts.Select(p => ReflectionModelServices.GetPartType(p).Value);
            }
            else if (catalog is AssemblyCatalog)
            {
                return ((AssemblyCatalog)catalog).Assembly.GetTypes();
            }
            else
            {
                return Enumerable.Empty<Type>();
            }
        }

        private static bool IsDeploymentCatalog(ComposablePartCatalog catalog)
        {
            //  Figure out if the catalog is a DeploymentCatalog by matching on the type name, in order to
            //  avoid adding a reference to the Composition.Initialization DLL (which only exists on Silverlight)
            return catalog.GetType().FullName == "System.ComponentModel.Composition.Hosting";
        }

        private Type GetTypeForPart(ComposablePartDefinition definition)
        {
            Type ret;
            if (_partToType.TryGetValue(definition, out ret))
            {
                return ret;
            }

            try
            {
                ret = ReflectionModelServices.GetPartType(definition).Value;
            }
            catch (ArgumentException)
            {
                //  ReflectionModelServices.GetPartType throws an argument exception if the definition passed in isn't a reflection part
                ret = null;
            }
            _partToType[definition] = ret;
            return ret;
        }

        private ComposablePartDefinition GetPartForType(Type type)
        {
            ComposablePartDefinition ret;
            if (_typeToPart.TryGetValue(type, out ret))
            {
                return ret;
            }

            ret = AttributedModelServices.CreatePartDefinition(type, new TypeCompositionElement(type), false);

            if (!ret.ExportDefinitions.Any() && !ret.ImportDefinitions.Any())
            {
                ret = null;
            }

            _typeToPart[type] = ret;
            return ret;
        }

        private class TypeCompositionElement : ICompositionElement
        {
            private Type _type;

            public TypeCompositionElement(Type type)
            {
                _type = type;
            }

            public string DisplayName
            {
                get { return _type.AssemblyQualifiedName; }
            }

            public ICompositionElement Origin
            {
                get { return null; }
            }
        }


        /// <summary>
        /// Retrieve analysis of a specific part definition given its (attributed)
        /// implementation type.
        /// </summary>
        /// <param name="attributedPartType">The implementation type of the part definition.</param>
        /// <returns>Analysis of the part definition.</returns>
        /// <remarks>The part must be a member of the catalog being analyzed.</remarks>
        public PartDefinitionInfo GetPartDefinitionInfo(Type attributedPartType)
        {
            return PartDefinitions
                .Where(pd => ReflectionModelServices.GetPartType(pd.PartDefinition).Value == attributedPartType)
                .SingleOrDefault();
        }

        /// <summary>
        /// Retrieve analysis of a specific part definition given the name of its
        /// (attributed) implementation type.
        /// </summary>
        /// <param name="typeName">The FullName of the implementation type of the part definition.</param>
        /// <returns>Analysis of the part definition.</returns>
        /// <remarks>The part must be a member of the catalog being analyzed.</remarks>
        public PartDefinitionInfo GetPartDefinitionInfo(string typeName)
        {
            return PartDefinitions
                .Where(pd => ReflectionModelServices.GetPartType(pd.PartDefinition).Value.FullName == typeName)
                .SingleOrDefault();
        }

        private IEnumerable<CompositionContractInfo> AnalyzeContracts()
        {
            var contracts = new Dictionary<string, CompositionContractInfo>();

            foreach (var pd in PartDefinitions)
            {
                foreach (var e in pd.PartDefinition.ExportDefinitions)
                {
                    CompositionContractInfo ci;
                    if (!contracts.TryGetValue(e.ContractName, out ci))
                        contracts[e.ContractName] = ci = new CompositionContractInfo(new ContractInfo(
                            e.ContractName,
                            TypeIdentity(e),
                            e.Metadata.Select(m =>
                                new KeyValuePair<string, Type>(m.Key, m.Value.GetType()))));
                    ci.Exporters.Add(pd);
                }

                foreach (var i in pd.ImportDefinitions)
                {
                    CompositionContractInfo ci;
                    if (!contracts.TryGetValue(i.ContractInfo.ContractName, out ci))
                        contracts[i.ContractInfo.ContractName] = ci = new CompositionContractInfo(i.ContractInfo);

                    ci.Importers.Add(pd);
                }
            }

            return contracts.Values;
        }

        static string TypeIdentity(ExportDefinition e)
        {
            object value;
            if (e.Metadata.TryGetValue(CompositionConstants.ExportTypeIdentityMetadataName, out value))
                return (string)value;
            else
                return null;
        }

        static ImportDefinitionInfo AnalyzeImportDefinition(ExportProvider host, IEnumerable<PartDefinitionInfo> availableParts, ImportDefinition id)
        {
            IEnumerable<ExportDefinition> actuals = Enumerable.Empty<ExportDefinition>();
            Exception exception = null;
            IEnumerable<UnsuitableExportDefinitionInfo> unsuitableExportDefinitions =
                Enumerable.Empty<UnsuitableExportDefinitionInfo>();

            try
            {
                actuals = host.GetExports(id).Select(e => e.Definition).ToArray();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var cbid = id as ContractBasedImportDefinition;
            if (cbid != null)
            {
                unsuitableExportDefinitions =
                    (from pd in availableParts
                    from ed in pd.PartDefinition.ExportDefinitions
                    where ed.ContractName == cbid.ContractName &&
                        !actuals.Contains(ed)
                    select new UnsuitableExportDefinitionInfo(cbid, ed, pd)).ToArray();
            }

            return new ImportDefinitionInfo(id, exception, actuals, unsuitableExportDefinitions);
        }
    }
}
