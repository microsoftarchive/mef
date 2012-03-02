// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    class ExportDescriptorRegistryUpdate : DependencyAccessor
    {
        readonly IDictionary<Contract, ExportDescriptor[]> _partDefinitions;
        readonly ExportDescriptorProvider[] _exportDescriptorProviders;
        readonly IDictionary<Contract, UpdateResult> _updateResults = new Dictionary<Contract, UpdateResult>();

        static readonly Dependency[] NoDependenciesValue = new Dependency[0];
        static readonly Func<Dependency[]> NoDependencies = () => NoDependenciesValue;

        bool _updateFinished;

        public ExportDescriptorRegistryUpdate(
            IDictionary<Contract, ExportDescriptor[]> partDefinitions,
            ExportDescriptorProvider[] exportDescriptorProviders)
        {
            _partDefinitions = partDefinitions;
            _exportDescriptorProviders = exportDescriptorProviders;
        }

        public void Execute(Contract contract)
        {
            // Opportunism - we'll miss recursive calls to Execute(), but this will catch some problems
            // and the _updateFinished flag is required for other purposes anyway.
            if (_updateFinished) throw new InvalidOperationException("The update has already executed.");

            Dependency initial;
            if (TryResolveOptionalDependency("initial request", contract, true, out initial))
            {
                var @checked = new HashSet<ExportDescriptorPromise>();
                var checking = new Stack<Dependency>();
                CheckTarget(initial, @checked, checking);
            }

            _updateFinished = true;

            foreach (var result in _updateResults)
            {
                var resultContract = result.Key;
                var descriptors = result.Value.GetResults().Select(cb => cb.GetDescriptor()).ToArray();
                _partDefinitions.Add(resultContract, descriptors);
            }
        }

        void CheckTarget(Dependency dependency, HashSet<ExportDescriptorPromise> @checked, Stack<Dependency> checking)
        {
            if (dependency.IsError)
            {
                var message = new StringBuilder();
                dependency.DescribeError(message);
                message.AppendLine();
                message.Append(DescribeCompositionStack(dependency, checking));
                message.Append(".");

                throw new LightweightCompositionException(message.ToString());
            }

            if (@checked.Contains(dependency.Target))
                return;

            @checked.Add(dependency.Target);

            checking.Push(dependency);
            foreach (var dep in dependency.Target.Dependencies)
                CheckDependency(dep, @checked, checking);

            checking.Pop();
        }

        void CheckDependency(Dependency dependency, HashSet<ExportDescriptorPromise> @checked, Stack<Dependency> checking)
        {
            if (@checked.Contains(dependency.Target))
            {
                var sharedSeen = false;
                var nonPrereqSeen = !dependency.IsPrerequisite;

                foreach (var step in checking)
                {
                    if (step.Target.IsShared)
                        sharedSeen = true;

                    if (sharedSeen && nonPrereqSeen)
                        break;

                    if (step.Target.Equals(dependency.Target))
                    {
                        var message = new StringBuilder();
                        message.AppendFormat("Importing part '{0}' creates an unsupported cycle{1}", dependency.Target.Origin, Environment.NewLine);                        
                        message.Append(DescribeCompositionStack(dependency, checking));
                        message.AppendLine(".");
                        message.Append("To construct a cycle, at least one part in the cycle must be shared, and at least one import in the cycle must be non-prerequisite (e.g. a property).");

                        throw new LightweightCompositionException(message.ToString());
                    }

                    if (!step.IsPrerequisite)
                        nonPrereqSeen = true;
                }
            }

            CheckTarget(dependency, @checked, checking);
        }

        StringBuilder DescribeCompositionStack(Dependency top, Stack<Dependency> stack)
        {
            var copy = new Stack<Dependency>(stack.Reverse());
            copy.Push(top);
            return DescribeCompositionStack(copy);
        }

        StringBuilder DescribeCompositionStack(Stack<Dependency> stack)
        {
            var result = new StringBuilder();
            if (stack.Count == 0)
                return result;

            Dependency import = null;
            foreach (var step in stack)
            {
                if (import == null)
                {
                    import = step;
                    continue;
                }

                result.AppendFormat(" -> required by import '{0}' of part '{1}'{2}", import.Site, step.Target.Origin, Environment.NewLine);
                import = step;
            }

            result.AppendFormat(" -> required by initial request for contract '{0}'", import.Contract);
            return result;
        }

        protected override ExportDescriptorPromise[] GetPromises(Contract contract)
        {
            if (_updateFinished)
                throw new InvalidOperationException("Update is finished - dependencies should have been requested earlier.");

            ExportDescriptor[] definitions;
            if (_partDefinitions.TryGetValue(contract, out definitions))
                return definitions.Select(d => new ExportDescriptorPromise(contract, "Preexisting", false, NoDependencies, _ => d)).ToArray();

            UpdateResult updateResult;
            if (!_updateResults.TryGetValue(contract, out updateResult))
            {
                updateResult = new UpdateResult(_exportDescriptorProviders);
                _updateResults.Add(contract, updateResult);
            }

            ExportDescriptorProvider nextProvider;
            while (updateResult.TryDequeueNextProvider(out nextProvider))
            {
                var newDefinitions = nextProvider.GetExportDescriptors(contract, this);
                foreach (var definition in newDefinitions)
                    updateResult.AddPromise(definition);
            }

            return updateResult.GetResults();
        }
    }
}
