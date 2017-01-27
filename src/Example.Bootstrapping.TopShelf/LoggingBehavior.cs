using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.TopShelf
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            this.Log().Debug($"Handling {typeof(TRequest).Name}");
            var response = await next();
            this.Log().Debug($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }
}