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

        private static IMediator CreateScopedMediator(IComponentContext scope)
        {
            var mediator = new Mediator(
                type => scope.IsRegistered(type) ? scope.Resolve(type) : null,
                type => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(type)));
            return mediator;
        }
    }
}