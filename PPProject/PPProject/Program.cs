using PPProject.Auth;
using PPProject.Common.Session;
using PPProject.Infrastructure;
using PPProject.Infrastructure.Mysql;
using PPProject.Middleware;
using PPProject.Middleware.Logger;
using PPProject.Profile;
using PPProject.Resource;
using PPProject.Usecase;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

//.net이 멋대로 JWT 이름을 바꾸는 기능을 끈다
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//.env 파일 로드
DotNetEnv.Env.Load();

//Selilog 추가
builder.AddSerilogLogging();
builder.Logging.AddConsole();


builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMysql(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddSnowflake();

//common
builder.Services.AddSingleton<RedisGameSessionStore>();
builder.Services.AddUsecases();

//Controller
builder.Services.AddAuth();
builder.Services.AddProfile();
builder.Services.AddResource();

//Middleware DL등록
builder.Services.AddScoped<IResponseHandler, ResponseEncryptionHandler>();
builder.Services.AddScoped<IResponseHandler, ResponseLoggingHandler>();

var app = builder.Build();


//MiddleWare
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseMiddleware<ExceptionLoggingMiddleware>();
}

app.UseMiddleware<RequestDecryptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GameSessionMiddleware>();

app.UseMiddleware<ResponsePipelineMiddleware>();




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
