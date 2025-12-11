using PPProject.Auth;
using PPProject.Infrastructure;
using PPProject.Middleware;

var builder = WebApplication.CreateBuilder(args);

//.env 파일 로드
DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMysql(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddSnowflake();

builder.Services.AddAuth();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseMiddleware<PacketEncryptionMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
