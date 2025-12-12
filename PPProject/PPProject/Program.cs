using PPProject.Auth;
using PPProject.Common.Session;
using PPProject.Infrastructure;
using PPProject.Middleware;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

//.net이 멋대로 JWT 이름을 바꾸는 기능을 끈다
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//.env 파일 로드
DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMysql(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddSnowflake();

//common
builder.Services.AddSingleton<RedisGameSessionStore>();

//Controller
builder.Services.AddAuth();

var app = builder.Build();


//MiddleWare
app.UseMiddleware<GameSessionMiddleware>();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();
else
    app.UseMiddleware<PacketEncryptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
