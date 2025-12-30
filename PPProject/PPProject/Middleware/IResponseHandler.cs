namespace PPProject.Middleware
{
    public interface IResponseHandler
    {
        Task HandleAsync(HttpContext context, string plainResponse); 
    }
}
