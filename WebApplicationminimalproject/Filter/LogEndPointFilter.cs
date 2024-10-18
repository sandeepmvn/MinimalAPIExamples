
namespace WebApplicationminimalproject.Filter
{
    public class LogEndPointFilter : IEndpointFilter
    {
        readonly ILogger<LogEndPointFilter> _Logger;
        public LogEndPointFilter(ILogger<LogEndPointFilter> Logger)
        {
            _Logger = Logger;
        }
        //Task<object?>
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            //before
            foreach (var item in context.Arguments)
            {
                _Logger.LogInformation(item.ToString());
            }
            _Logger.LogInformation("Before");
            var result = await next(context);
            //after
            _Logger.LogInformation("After");
            return result;
        }
    }
}
