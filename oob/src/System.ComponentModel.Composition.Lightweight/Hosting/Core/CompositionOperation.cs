// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Threading;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Core
{
    /// <summary>
    /// Represents a single logical graph-building operation.
    /// </summary>
    /// <remarks>Instances of this class are not safe for access by multiple threads.</remarks>
    public sealed class CompositionOperation : IDisposable
    {
        List<Action> _nonPrerequisiteActions;
        List<Action> _postCompositionActions;
        Stack<object> _sharingLocks;

        // Construct using Run() method.
        CompositionOperation() { }

        /// <summary>
        /// Execute a new composition operation starting within the specified lifetime
        /// context, for the specified activator.
        /// </summary>
        /// <param name="context">Context in which to begin the operation (the operation can flow
        /// to the parents of the context if requried).</param>
        /// <param name="activator">Activator that will drive the operation.</param>
        /// <returns>The composed object graph.</returns>
        public static object Run(LifetimeContext context, CompositeActivator activator)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (activator == null) throw new ArgumentNullException("activator");

            using (var operation = new CompositionOperation())
            {
                var result = activator(context, operation);
                operation.Complete();
                return result;
            }
        }

        /// <summary>
        /// Called during the activation process to specify an action that can run after all
        /// prerequesite part dependencies have been satisfied.
        /// </summary>
        /// <param name="action">Action to run.</param>
        public void AddNonPrerequisiteAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (_nonPrerequisiteActions == null)
                _nonPrerequisiteActions = new List<Action>();

            _nonPrerequisiteActions.Add(action);
        }

        /// <summary>
        /// Called during the activation process to specify an action that must run only after
        /// all composition has completed. See <see cref="IPartImportsSatisfiedNotification"/>.
        /// </summary>
        /// <param name="action">Action to run.</param>
        public void AddPostCompositionAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (_postCompositionActions == null)
                _postCompositionActions = new List<Action>();

            _postCompositionActions.Add(action);
        }

        internal void EnterSharingLock(object sharingLock)
        {
            if (_sharingLocks == null)
                _sharingLocks = new Stack<object>();

            _sharingLocks.Push(sharingLock);
            Monitor.Enter(sharingLock);
        }

        void Complete()
        {
            while (_nonPrerequisiteActions != null)
                RunAndClearActions();

            if (_postCompositionActions != null)
            {
                foreach (var action in _postCompositionActions)
                    action();

                _postCompositionActions = null;
            }
        }

        void RunAndClearActions()
        {
            var currentActions = _nonPrerequisiteActions;
            _nonPrerequisiteActions = null;

            foreach (var action in currentActions)
                action();
        }

        /// <summary>
        /// Release locks held during the operation.
        /// </summary>
        public void Dispose()
        {
            if (_sharingLocks != null)
            {
                while (_sharingLocks.Count != 0)
                    Monitor.Exit(_sharingLocks.Pop());
            }
        }
    }
}
