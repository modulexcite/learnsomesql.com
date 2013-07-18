namespace RedGate.Publishing.InteractiveSql
{
    public class SqlServerUser
    {
        private readonly string m_Username;
        private readonly string m_Password;

        public SqlServerUser(string username, string password)
        {
            m_Username = username;
            m_Password = password;
        }

        public string Username
        {
            get { return m_Username; }
        }

        public string Password
        {
            get { return m_Password; }
        }
    }
}