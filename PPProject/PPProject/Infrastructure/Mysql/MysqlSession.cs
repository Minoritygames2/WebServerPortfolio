using MySqlConnector;

namespace PPProject.Infrastructure.Mysql
{
    public class MysqlSession
    {
        public MySqlConnection Connection { get; }
        public MySqlTransaction? Transaction { get; private set; }
        public MysqlSession(MySqlConnection connection)
        {
            Connection = connection;
            if (Connection.State != System.Data.ConnectionState.Open)
                Connection.Open();
        }

        public void BeginTransaction()
        {
            Transaction ??= Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
            Transaction?.Dispose();
            Transaction = null;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction?.Dispose();
            Transaction = null;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection.Dispose();
        }
    }
}
