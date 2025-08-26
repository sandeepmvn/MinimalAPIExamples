
namespace WebApplicationminimalproject.Filter
{
    public class LogEndPointFilter : IEndpointFilter
    {
        readonly ILogger<LogEndPointFilter> _Logger;
        public LogEndPointFilter(ILogger<LogEndPointFilter> Logger)
        {
            _Logger = Logger;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Log arguments before execution
            foreach (var item in context.Arguments)
            {
                _Logger.LogInformation(item?.ToString() ?? "null");
            }
            _Logger.LogInformation("Before");
            var result = await next(context);
            // Log after execution
            _Logger.LogInformation("After");
            return result;
        }
    }
}
