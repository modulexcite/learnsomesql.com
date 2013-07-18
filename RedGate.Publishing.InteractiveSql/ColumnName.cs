namespace RedGate.Publishing.InteractiveSql
{
    public class ColumnName
    {
        private readonly string m_Name;

        public static ColumnName Create(string name)
        {
            return new ColumnName(name);
        }

        private ColumnName(string name)
        {
            m_Name = name;
        }

        public override string ToString()
        {
            return m_Name;
        }

        protected bool Equals(ColumnName other)
        {
            return string.Equals(m_Name.ToLowerInvariant(), other.m_Name.ToLowerInvariant());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColumnName) obj);
        }

        public override int GetHashCode()
        {
            return m_Name.ToLowerInvariant().GetHashCode();
        }
    }
}
