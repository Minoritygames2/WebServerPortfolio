
using StackExchange.Redis;

namespace PPProject.Infrastructure
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
        {
            var host = config["REDIS_HOST"];
            var port = config["PORT"];
            var password = config["PASSWORD"];

            string conn;
            if (string.IsNullOrWhiteSpace(password))
                conn = $"{host}:{port},allowAdmin=true";
            else
                conn = $"{host}:{port},password={password},allowAdmin=true";

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(conn);
            });

            services.AddScoped<IDatabase>(sp => 
            {
                var mux = sp.GetRequiredService<IConnectionMultiplexer>();
                return mux.GetDatabase();
            });

            return services;
        }
    }
}
