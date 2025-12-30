using PPProject.Common;
using System.Collections;
using System.Text;

namespace PPProject.Middleware
{
    public class ResponsePipelineMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponsePipelineMiddleware(
                RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var handlers = context.RequestServices.GetRequiredService<IEnumerable<IResponseHandler>>();

            var originalBody = context.Response.Body;

            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            await _next(context);

            buffer.Position = 0;
            var plainResponse = await new StreamReader(buffer).ReadToEndAsync();

            foreach(var handler in handlers)
            {
                await handler.HandleAsync(context, plainResponse);
            }

            var finalResponse =
                context.Items.TryGetValue(Define.FINAL_RES_BODY, out var final)
                ? final.ToString()
                : plainResponse;

            context.Response.Body = originalBody;
            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(finalResponse));
        }
    }
}
