namespace PPProject.Common.Extensions
{
    public static class HttpContextExtensions
    {
        private const string UserIdKey = "uId";
        public static long GetUserId(this HttpContext context)
        {
            if (context.Items.TryGetValue(UserIdKey, out var userIdObj) && userIdObj is long userId)
            {
                return userId;
            }
            throw new InvalidOperationException("User ID not found in HttpContext items.");
        }
    }
}
