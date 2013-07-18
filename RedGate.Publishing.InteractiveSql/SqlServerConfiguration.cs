using System.Configuration;

namespace RedGate.Publishing.InteractiveSql
{
    public class SqlServerConfiguration
    {
        public static SqlServerConfiguration ReadAppSettings()
        {
            return new SqlServerConfiguration(
                ConfigurationManager.AppSettings["SqlServerInstance"],
                ConfigurationManager.AppSettings["SqlServerDatabaseName"],
                new SqlServerUser(
                    ConfigurationManager.AppSettings["SqlServerAdminUsername"],
                    ConfigurationManager.AppSettings["SqlServerAdminPassword"]
                ),
                new SqlServerUser(
                    ConfigurationManager.AppSettings["SqlServerUnprivilegedUsername"],
                    ConfigurationManager.AppSettings["SqlServerUnprivilegedPassword"]
                )
            );
        }

        private readonly string m_InstanceName;
        private readonly string m_DatabaseName;
        private readonly SqlServerUser m_AdminUser;
        private readonly SqlServerUser m_UnprivilegedUser;

        public SqlServerConfiguration(string instanceName, string databaseName, SqlServerUser adminUser, SqlServerUser unprivilegedUser)
        {
            m_InstanceName = instanceName;
            m_DatabaseName = databaseName;
            m_AdminUser = adminUser;
            m_UnprivilegedUser = unprivilegedUser;
        }

        public string DatabaseName
        {
            get { return m_DatabaseName; }
        }

        public string UnprivilegedConnectionString()
        {
            return ConnectionStringForUser(m_UnprivilegedUser);
        }

        public string AdminConnectionString()
        {
            return ConnectionStringForUser(m_AdminUser);
        }

        private string ConnectionStringForUser(SqlServerUser user)
        {
            return string.Format(
                "Data Source={0};Initial Catalog={1};User ID={2};Password={3};",
                m_InstanceName,
                m_DatabaseName,
                user.Username,
                user.Password
            );
        }
    }
}
