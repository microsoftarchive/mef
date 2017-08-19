//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.ComponentModel.Composition.Diagnostics;
using System.IO;
using mefx.CommandLine;

namespace mefx
{
    enum Command
    {
        None,
        PrintParts,
        PrintContracts,
        PrintUsage
    }

    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var directories = new List<string>();
                var files = new List<string>();

                var programCommand = Command.None;
                Func<CompositionContractInfo, bool> contractPredicate = c => false;
                Func<CompositionInfo, PartDefinitionInfo, bool> partPredicate = (ci, p) => false;
                var verbose = false;
                var whitelist = new RejectionWhitelist();

                var opts = new Options();
                
                opts.Add<string>("dir", @"C:\MyApp\Parts", "Specify directories to search for parts.",
                    d => directories.Add(d));
                opts.Add<string>("file", "MyParts.dll", "Specify assemblies to search for parts.",
                    f => files.Add(f));
                opts.AddSwitch("verbose", "Print verbose information on each part.",
                    () => verbose = true);

                var programCommandGroup = new ExclusiveGroup();

                opts.Add<string>("type", "MyNamespace.MyType", "Print details of the given part type.", t => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => p == ci.GetPartDefinitionInfo(t));
                    },
                    programCommandGroup);

                opts.Add<string>("importers", "MyContract", "List importers of the given contract.", i => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => p.ImportsContract(i));
                    },
                    programCommandGroup);

                opts.Add<string>("exporters", "MyContract", "List exporters of the given contract.", e => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => p.ExportsContract(e));
                    },
                    programCommandGroup);

                opts.AddSwitch("rejected", "List all rejected parts.", () => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => p.IsRejected);
                    },
                    programCommandGroup);

                opts.AddSwitch("causes", "List root causes - parts with errors not related to the rejection of other parts.", () => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => p.IsPrimaryRejection);
                    },
                    programCommandGroup);

                opts.Add<string>("whitelist", "RejectionWhitelist.txt", "Specify parts that may be validly rejected; requres the /rejected or /causes commands.",
                    w => whitelist = new RejectionWhitelist(w));

                opts.AddSwitch("parts", "List all parts found in the source assemblies.", () => {
                        programCommand = Command.PrintParts;
                        partPredicate = AddToPredicate(partPredicate, (ci, p) => true);
                    },
                    programCommandGroup);

                opts.AddSwitch("?", "Print usage.",
                    () => programCommand = Command.PrintUsage, programCommandGroup);

                var contractsSubgroup = new InclusiveSubroup(programCommandGroup);
                opts.AddSwitch("imports", "Find imported contracts.", () => {
                        programCommand = Command.PrintContracts;
                        contractPredicate = AddToPredicate(contractPredicate, c => c.Importers.Any());
                    },
                    contractsSubgroup);

                opts.AddSwitch("exports", "Find exported contracts.", () => {
                        programCommand = Command.PrintContracts;
                        contractPredicate = AddToPredicate(contractPredicate, c => c.Exporters.Any());
                    },
                    contractsSubgroup);

                opts.Parse(args);

                return Run(directories, files, programCommand, contractPredicate, partPredicate, verbose, whitelist, opts);
            }
            catch (Exception ex)
            {
                Console.Write("Error: ");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        static int Run(
            List<string> directories,
            List<string> files, 
            Command programCommand, 
            Func<CompositionContractInfo, bool> contractPredicate, 
            Func<CompositionInfo, PartDefinitionInfo, bool> partPredicate, 
            bool verbose, RejectionWhitelist whitelist, Options opts)
        {
            int exitCode = 0;

            if (programCommand == Command.None ||
                programCommand == Command.PrintUsage ||
                !files.Concat(directories).Any())
            {
                PrintUsage(opts);
            }
            else
            {
                var test = new AssemblyCatalog(@"C:\Users\daplaist\Documents\My Dropbox\MEF\Tech Ed 2010\MEFConsole\bin\Debug\MEFConsole.exe");

                var cat = new AggregateCatalog(
                    directories.Select(d => new DirectoryCatalog(d)).Cast<ComposablePartCatalog>()
                    .Concat(directories.Select(d => new DirectoryCatalog(d, "*.exe")).Cast<ComposablePartCatalog>())
                    .Concat(files.Select(f => new AssemblyCatalog(f)).Cast<ComposablePartCatalog>()));

                var container = new CompositionContainer(cat);
                var compositionInfo = new CompositionInfo(cat, container);

                if (programCommand == Command.PrintParts)
                {
                    var partsToPrint = compositionInfo.PartDefinitions
                        .Where(p => partPredicate(compositionInfo, p));

                    foreach (var pdi in partsToPrint)
                    {
                        if (pdi.IsRejected &&
                            !whitelist.IsRejectionAllowed(pdi))
                        {
                            Console.Write("[Unexpected] ");
                            exitCode = 1;
                        }
                        PrintPartDefinition(pdi, Console.Out, verbose);
                    }
                }
                else if (programCommand == Command.PrintContracts)
                {
                    PrintContracts(compositionInfo.Contracts.Where(contractPredicate));
                }
            }
            return exitCode;
        }

        static void PrintUsage(Options opts)
        {
            var v = typeof(Program).Assembly.GetName().Version;
            Console.WriteLine("Managed Extensibility Framework Explorer Version {0}", v);
            Console.WriteLine("Copyright (c) 2009 Microsoft Corporation. All rights reserved.");
            Console.WriteLine();
            Console.WriteLine("Built for: {0}", typeof(Export).Assembly);
            Console.WriteLine();
            Console.WriteLine("Usage:     mefx.exe [assembly files and directories] [action] {options}");
            Console.WriteLine();
            Console.WriteLine("Example:   mefx.exe /file:MyAssembly.dll /causes /verbose");
            Console.WriteLine();
            Console.WriteLine("Switches:");
            Console.WriteLine();
            opts.PrintUsage(Console.Out);
        }

        static void PrintPartDefinition(PartDefinitionInfo pdi, TextWriter writer, bool verbose)
        {
            if (verbose)
            {
                PartDefinitionInfoTextFormatter.Write(pdi, writer);
                writer.WriteLine();
            }
            else
            {
                writer.WriteLine(CompositionElementTextFormatter.DisplayCompositionElement(pdi.PartDefinition));
            }
        }

        static void PrintContracts(IEnumerable<CompositionContractInfo> contracts)
        {
            foreach (var c in contracts)
                Console.WriteLine(c.Contract.DisplayString);
        }

        static Func<CompositionInfo, PartDefinitionInfo, bool> AddToPredicate(
            Func<CompositionInfo, PartDefinitionInfo, bool> existing,
            Func<CompositionInfo, PartDefinitionInfo, bool> addition)
        {
            return (ci, pi) => existing(ci, pi) || addition(ci, pi);
        }

        static Func<CompositionContractInfo, bool> AddToPredicate(
            Func<CompositionContractInfo, bool> existing,
            Func<CompositionContractInfo, bool> addition)
        {
            return c => existing(c) || addition(c);
        }
    }
}
