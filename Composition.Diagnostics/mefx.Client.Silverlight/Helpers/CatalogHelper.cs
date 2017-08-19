// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

namespace mefx.Client.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Reflection;

    public class CatalogHelper
    {
        public static void DiscoverParts(AggregateCatalog catalog, IEnumerable<Assembly> assemblies)
        {
            //var addedDefinitions = new List<ComposablePartDefinition>();
            //var addedCatalogs = new Dictionary<string, ComposablePartCatalog>();

            //foreach (var assembly in assemblies)
            //{
            //    if (addedCatalogs.ContainsKey(assembly.FullName))
            //    {
            //        // Nothing to do because the assembly has already been added.
            //        continue;
            //    }

            //    var assemblyCatalog = new AssemblyCatalog(assembly);
            //    addedDefinitions.AddRange(assemblyCatalog.Parts);
            //    addedCatalogs.Add(assembly.FullName, assemblyCatalog);
            //}

            //using (var atomicComposition = new AtomicComposition())
            //{
            //    foreach (var item in addedCatalogs)
            //    {
            //        catalog.Catalogs.Add(item.Value);
            //    }
            //    atomicComposition.Complete();
            //}

            //var addedDefinitions = new List<ComposablePartDefinition>();
            //var addedCatalogs = new Dictionary<string, ComposablePartCatalog>();

            //foreach (var assembly in assemblies)
            //{
            //    if (addedCatalogs.ContainsKey(assembly.FullName))
            //    {
            //        // Nothing to do because the assembly has already been added.
            //        continue;
            //    }

            //    var assemblyCatalog = new AssemblyCatalog(assembly);
            //    addedDefinitions.AddRange(assemblyCatalog.Parts);
            //    addedCatalogs.Add(assembly.FullName, assemblyCatalog);
            //}

            using (var atomicComposition = new AtomicComposition())
            {
                foreach (var assembly in assemblies)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("{0}", assembly.FullName));

                    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                }

                //foreach (var item in addedCatalogs)
                //{
                //    catalog.Catalogs.Add(item.Value);
                //}
                atomicComposition.Complete();
            }
        }
    }
}