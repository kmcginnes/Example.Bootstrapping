using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.Console
{
    internal class ScopedMediator : IMediator
    {
        private readonly SingleInstanceFactory _singleInstanceFactory;
        private readonly MultiInstanceFactory _multiInstanceFactory;
        private readonly Func<IDisposable> _beginLifetimeScope;

        public ScopedMediator(
            SingleInstanceFactory singleInstanceFactory, 
            MultiInstanceFactory multiInstanceFactory, 
            Func<IDisposable> beginLifetimeScope)
        {
            _singleInstanceFactory = singleInstanceFactory;
            _multiInstanceFactory = multiInstanceFactory;
            _beginLifetimeScope = beginLifetimeScope;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_beginLifetimeScope())
            {
                var mediator = new Mediator(_singleInstanceFactory, _multiInstanceFactory);
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_beginLifetimeScope())
            {
                var mediator = new Mediator(_singleInstanceFactory, _multiInstanceFactory);
                return mediator.Send(request, cancellationToken);
            }
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            using (_beginLifetimeScope())
            {
                var mediator = new Mediator(_singleInstanceFactory, _multiInstanceFactory);
                return mediator.Publish(notification, cancellationToken);
            }
        }
    }
}