using System;
using System.Threading.Tasks;
using MediatR;

namespace Example.Bootstrapping.TopShelf
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var requestTypeName = typeof(TRequest).GetFriendlyName();
            using (request.Log().Time(requestTypeName))
            {
                try
                {
                    var serializedRequest = JsonConvert.SerializeObject(request);
                    request.Log().Debug($"Handling {requestTypeName} {serializedRequest}");
                    var response = await next();
                    var responseTypeName = response.GetType().GetFriendlyName();
                    var serializedResponse = JsonConvert.SerializeObject(response);
                    request.Log().Debug($"Handled {requestTypeName} with response {responseTypeName} {serializedResponse}");
                    return response;
                }
                catch (Exception exception)
                {
                    request.Log().Error(exception, $"Failed to handle {requestTypeName}");
                    throw;
                }
            }
        }
    }
}