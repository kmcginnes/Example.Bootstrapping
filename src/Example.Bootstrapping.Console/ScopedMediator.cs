using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.Console
{
    internal class ScopedMediator : IMediator
    {
        private readonly Func<IDisposable> _beginLifetimeScope;
        private readonly Mediator _mediator;

        public ScopedMediator(
            SingleInstanceFactory singleInstanceFactory, 
            MultiInstanceFactory multiInstanceFactory, 
            Func<IDisposable> beginLifetimeScope)
        {
            _beginLifetimeScope = beginLifetimeScope;
            _mediator = new Mediator(singleInstanceFactory, multiInstanceFactory);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_beginLifetimeScope())
            {
                return _mediator.Send(request, cancellationToken);
            }
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_beginLifetimeScope())
            {
                return _mediator.Send(request, cancellationToken);
            }
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            using (_beginLifetimeScope())
            {
                return _mediator.Publish(notification, cancellationToken);
            }
        }
    }
}