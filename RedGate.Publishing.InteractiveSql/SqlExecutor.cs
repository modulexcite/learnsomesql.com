using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RedGate.Publishing.InteractiveSql
{
    public class SqlExecutor
    {
        private readonly SqlServerConfiguration m_Configuration;

        public SqlExecutor(SqlServerConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        public IQueryResult ExecuteQuery(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return QueryResult.Failure("Query is empty");
            }
            try
            {
                return RunQuery(sql);
            }
            catch (SqlException exception)
            {
                return QueryResult.Failure(exception.Message);
            }
        }

        private IQueryResult RunQuery(string sql)
        {
            using (var connection = OpenUnprivilegedConnection())
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandTimeout = 1;
                    var reader = command.ExecuteReader();

                    var rows = new List<QueryResultRow>();

                    var columnNames = Enumerable.Range(0, reader.FieldCount)
                        .Select(reader.GetName)
                        .ToList();

                    while (reader.Read())
                    {
                        var values = Enumerable.Range(0, reader.FieldCount)
                            .Select(index => new QueryResultField(reader.GetDataTypeName(index), reader.GetValue(index)))
                            .ToList();
                        rows.Add(new QueryResultRow(values));
                    }

                    return QueryResult.Success(columnNames, rows);
                }
            }
        }

        public void SetUpDatabase(string creationSql)
        {
            using (var connection = OpenAdminConnection())
            {
                // TODO: need a more thorough way of dropping all tables (and other objects)
                ExecuteNonQuery(connection, "exec sp_MSforeachtable 'DROP TABLE ?'");
                ExecuteNonQuery(connection, creationSql);
            }
        }

        private void ExecuteNonQuery(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private SqlConnection OpenUnprivilegedConnection()
        {
            return OpenConnection(m_Configuration.UnprivilegedConnectionString());
        }

        private SqlConnection OpenAdminConnection()
        {
            return OpenConnection(m_Configuration.AdminConnectionString());
        }

        private SqlConnection OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
