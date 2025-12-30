namespace PPProject.Middleware.Logger
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(
            RequestDelegate requestDelegate,
            ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                       "{Method} {Path} {QueryString} | TraceId : {TraceId}",
                       context.Request.Method,
                       context.Request.Path,
                       context.Request.QueryString.Value,
                       context.TraceIdentifier
                    );

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        message = ex.Message
                    });
            }
        }
    }
}
