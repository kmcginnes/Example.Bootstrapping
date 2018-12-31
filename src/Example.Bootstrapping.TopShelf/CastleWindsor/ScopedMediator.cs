using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using MediatR;

namespace Example.Bootstrapping.TopShelf.CastleWindsor
{
    public class ScopedMediator : IMediator
    {
        private readonly IKernel _kernel;
        private readonly IMediator _mediator;

        public ScopedMediator(IKernel kernel)
        {
            _kernel = kernel;
            _mediator = new Mediator(t => kernel.Resolve(t));
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_kernel.BeginScope())
            {
                return _mediator.Send(request, cancellationToken);
            }
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (_kernel.BeginScope())
            {
                return _mediator.Send(request, cancellationToken);
            }
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            using (_kernel.BeginScope())
            {
                return _mediator.Publish(notification, cancellationToken);
            }
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (_kernel.BeginScope())
            {
                return _mediator.Publish(notification, cancellationToken);
            }
        }
    }
}
