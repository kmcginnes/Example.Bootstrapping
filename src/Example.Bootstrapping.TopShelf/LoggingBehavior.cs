using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;

namespace Example.Bootstrapping.TopShelf
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IDisposable
    {
        public LoggingBehavior()
        {
            this.Log().Debug($"{nameof(LoggingBehavior<TRequest, TResponse>)}.ctor()");
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            this.Log().Debug($"{nameof(LoggingBehavior<TRequest, TResponse>)}.Handle()");

            var requestTypeName = typeof(TRequest).GetFriendlyName();
            using (request.Log().Time(requestTypeName))
            {
                try
                {
                    var serializedRequest = JsonConvert.SerializeObject(request);
                    request.Log().Info($"Handling {requestTypeName} {serializedRequest}");
                    var response = await next();
                    var responseTypeName = response.GetType().GetFriendlyName();
                    var serializedResponse = JsonConvert.SerializeObject(response);
                    request.Log().Info($"Handled {requestTypeName} with response {responseTypeName} {serializedResponse}");
                    return response;
                }
                catch (Exception exception)
                {
                    request.Log().Error(exception, $"Failed to handle {requestTypeName}");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            this.Log().Debug($"{nameof(LoggingBehavior<TRequest, TResponse>)}.Dispose()");
        }
    }
}