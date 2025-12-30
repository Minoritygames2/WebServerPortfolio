
using PPProject.Common;
using System.Text.Json;

namespace PPProject.Middleware.Logger
{
    public class ResponseLoggingHandler : IResponseHandler
    {
        private readonly ILogger<ResponseLoggingHandler> _logger;

        public ResponseLoggingHandler(ILogger<ResponseLoggingHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(HttpContext context, string plainResponse)
        {
            var requestBody = context.Items.TryGetValue(Define.PLAIN_REQ_BODY, out var req)
                ? req?.ToString()
                : plainResponse;


            var logFormat = "HTTP {Method} {Path} | {StatusCode} |  Request : {Request} | Response: {Response}";

            _logger.LogInformation(
                logFormat,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                OnLineJson(requestBody),
                OnLineJson(plainResponse));

            return Task.CompletedTask;
        }

        private string OnLineJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            try
            {
                var obj = JsonSerializer.Deserialize<object>(json);
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return json.Replace("\r", "").Replace("\n", "").Trim();
            }
        }
    }
}
