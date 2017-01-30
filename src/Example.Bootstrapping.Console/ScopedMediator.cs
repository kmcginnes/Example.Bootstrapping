using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;

namespace Example.Bootstrapping.Console
{
    internal class ScopedMediator : IMediator
    {
        private readonly Func<ILifetimeScope> _createScope;

        public ScopedMediator(Func<ILifetimeScope> createScope)
        {
            _createScope = createScope;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var scope = _createScope())
            {
                var mediator = CreateScopedMediator(scope);
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var scope = _createScope())
            {
                var mediator = CreateScopedMediator(scope);
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            using (var scope = _createScope())
            {
                var mediator = CreateScopedMediator(scope);
                return mediator.Publish(notification, cancellationToken);
            }
        }

        private IMediator CreateScopedMediator(ILifetimeScope scope)
        {
            var mediator = new Mediator(
                t => scope.Resolve(t),
                t => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(t)));
            return mediator;
        }
    }
}