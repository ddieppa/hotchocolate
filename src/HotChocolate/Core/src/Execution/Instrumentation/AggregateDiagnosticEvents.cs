using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Resolvers;

namespace HotChocolate.Execution.Instrumentation
{
    internal sealed class AggregateDiagnosticEvents : IDiagnosticEvents
    {
        private static EmptyActivityScope _empty = new EmptyActivityScope();
        private readonly IDiagnosticEventListener[] _listeners;
        private readonly IDiagnosticEventListener[] _resolverListener;

        public AggregateDiagnosticEvents(IDiagnosticEventListener[] listeners)
        {
            _listeners = listeners;
            _resolverListener = listeners.Where(t => t.EnableResolveFieldValue).ToArray();
        }

        public IActivityScope ExecuteRequest(IRequestContext context)
        {
            var scopes = new IActivityScope[_listeners.Length];

            for (int i = 0; i < _listeners.Length; i++)
            {
                scopes[i] = _listeners[i].ExecuteRequest(context);
            }

            return new AggregateActivityScope(scopes);
        }

        public void RequestError(IRequestContext context, Exception exception)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].RequestError(context, exception);
            }
        }

        public IActivityScope ParseDocument(IRequestContext context)
        {
            var scopes = new IActivityScope[_listeners.Length];

            for (int i = 0; i < _listeners.Length; i++)
            {
                scopes[i] = _listeners[i].ParseDocument(context);
            }

            return new AggregateActivityScope(scopes);
        }

        public void SyntaxError(IRequestContext context, IError error)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].SyntaxError(context, error);
            }
        }

        public IActivityScope ValidateDocument(IRequestContext context)
        {
            var scopes = new IActivityScope[_listeners.Length];

            for (int i = 0; i < _listeners.Length; i++)
            {
                scopes[i] = _listeners[i].ValidateDocument(context);
            }

            return new AggregateActivityScope(scopes);
        }

        public void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].ValidationErrors(context, errors);
            }
        }

        public IActivityScope ResolveFieldValue(IMiddlewareContext context)
        {
            if (_listeners.Length == 0)
            {
                return _empty;
            }

            var scopes = new IActivityScope[_resolverListener.Length];

            for (int i = 0; i < _resolverListener.Length; i++)
            {
                scopes[i] = _resolverListener[i].ResolveFieldValue(context);
            }

            return new AggregateActivityScope(scopes);
        }

        public void ResolverError(IMiddlewareContext context, IError error)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].ResolverError(context, error);
            }
        }

        public IActivityScope RunTask(IExecutionTask task)
        {
            if (_listeners.Length == 0)
            {
                return _empty;
            }

            var scopes = new IActivityScope[_resolverListener.Length];

            for (int i = 0; i < _resolverListener.Length; i++)
            {
                scopes[i] = _resolverListener[i].RunTask(task);
            }

            return new AggregateActivityScope(scopes);
        }

        public void TaskError(IExecutionTask task, IError error)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].TaskError(task, error);
            }
        }

        public void AddedDocumentToCache(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].AddedDocumentToCache(context);
            }
        }

        public void RetrievedDocumentFromCache(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].RetrievedDocumentFromCache(context);
            }
        }

        public void RetrievedDocumentFromStorage(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].RetrievedDocumentFromStorage(context);
            }
        }

        public void AddedOperationToCache(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].AddedDocumentToCache(context);
            }
        }

        public void RetrievedOperationFromCache(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].RetrievedDocumentFromCache(context);
            }
        }

        public void BatchDispatched(IRequestContext context)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].BatchDispatched(context);
            }
        }

        public void ExecutorCreated(string name, IRequestExecutor executor)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].ExecutorCreated(name, executor);
            }
        }

        public void ExecutorEvicted(string name, IRequestExecutor executor)
        {
            for (int i = 0; i < _listeners.Length; i++)
            {
                _listeners[i].ExecutorEvicted(name, executor);
            }
        }

        private class AggregateActivityScope : IActivityScope
        {
            private readonly IActivityScope[] _scopes;
            private bool _disposed;

            public AggregateActivityScope(IActivityScope[] scopes)
            {
                _scopes = scopes;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    for (int i = 0; i < _scopes.Length; i++)
                    {
                        _scopes[i].Dispose();
                    }
                    _disposed = true;
                }
            }
        }

        private class EmptyActivityScope : IActivityScope
        {
            public void Dispose()
            {
            }
        }
    }
}
