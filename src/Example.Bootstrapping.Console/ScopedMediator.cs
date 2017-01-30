using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;

namespace Example.Bootstrapping.Console
{
    internal class ScopedMediator : IMediator
    {
        private readonly IContainer _kernel;

        public ScopedMediator(IContainer kernel)
        {
            _kernel = kernel;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var scope = _kernel.BeginLifetimeScope())
            {
                var mediator = new Mediator(
                    t => scope.Resolve(t),
                    t => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(t)));
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var scope = _kernel.BeginLifetimeScope())
            {
                var mediator = new Mediator(
                    t => scope.Resolve(t),
                    t => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(t)));
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            using (var scope = _kernel.BeginLifetimeScope())
            {
                var mediator = new Mediator(
                    t => scope.Resolve(t),
                    t => (IEnumerable<object>)scope.Resolve(typeof(IEnumerable<>).MakeGenericType(t)));
                return mediator.Publish(notification, cancellationToken);
            }
        }
    }
}