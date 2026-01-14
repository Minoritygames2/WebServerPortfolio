using Dapper;
using MySqlConnector;
using PPProject.Infrastructure.Mysql.Handler;

namespace PPProject.Infrastructure.Mysql
{
    public static class MysqlExtensions
    {
        public static IServiceCollection AddMysql(this IServiceCollection service, IConfiguration config)
        {
            var host = config["MYSQL_HOST"];
            var port = config["MYSQL_PORT"];
            var user = config["MYSQL_USER"];
            var pw = config["MYSQL_PASSWORD"];
            var db = config["MYSQL_DB"];

            var connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pw};";
            service.AddScoped<MySqlConnection>(_ =>
                new MySqlConnection(connectionString)
            );

            service.AddScoped<MysqlSession>();

            //Handler 등록
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

            return service;
        }
    }
}
