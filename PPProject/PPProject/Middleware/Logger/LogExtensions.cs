using Serilog;
using Serilog.Events;

namespace PPProject.Middleware.Logger
{
    public static class LogExtensions
    {
        public static void AddSerilogLogging(this WebApplicationBuilder builder)
        {
            var logPath = Path.Combine(
                AppContext.BaseDirectory,
                "Logs/api",
                "api-.log"
                );

            var errorPath = Path.Combine(
                AppContext.BaseDirectory,
                "Logs/error",
                "error-.log"
                );


            Console.WriteLine(" == Log를 작성합니다 :: " + logPath);

            var logger = new LoggerConfiguration()
                .MinimumLevel.Information() // 전체 로그 필터
                //로그 파일
                .WriteTo.Logger(loggerConfig => 
                    loggerConfig
                    // ApiLoggingMiddleware에서 나온 로그만 작성
                    .Filter.ByIncludingOnly(e => 
                        e.Properties.ContainsKey("SourceContext") 
                        && e.Properties["SourceContext"].ToString()
                            .Contains("ResponseLoggingHandler"))
                    .WriteTo.File(
                        logPath, // 파일명
                        rollingInterval: RollingInterval.Day, //일 단위 작성
                        restrictedToMinimumLevel: LogEventLevel.Information // 현재 파일 필터
                    )
                )

                //에러 파일
                .WriteTo.File(
                    errorPath, // 파일명
                    rollingInterval: RollingInterval.Day, //일 단위 작성
                    restrictedToMinimumLevel: LogEventLevel.Error // 현재 파일 필터
                )
                
                .CreateLogger();

            builder.Host.UseSerilog(logger, dispose: true);
        }
    }
}
