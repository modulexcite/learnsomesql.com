using System;

namespace RedGate.Publishing.InteractiveSql
{
    public class QueryResultField : IEquatable<QueryResultField>
    {
        private readonly string m_SqlDataTypeName;
        private readonly object m_Value;

        public QueryResultField(string sqlDataTypeName, object value)
        {
            m_SqlDataTypeName = sqlDataTypeName;
            m_Value = value;
        }

        public object Value
        {
            get { return m_Value; }
        }

        public string DisplayValue()
        {
            if (m_SqlDataTypeName == "date")
            {
                return ((DateTime)m_Value).ToString("yyy-MM-dd");
            }
            else
            {
                return m_Value.ToString();
            }
        }

        public bool Equals(QueryResultField other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(m_SqlDataTypeName, other.m_SqlDataTypeName) && m_Value.Equals(other.m_Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryResultField) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_SqlDataTypeName.GetHashCode() * 397) ^ m_Value.GetHashCode();
            }
        }

        public static bool operator ==(QueryResultField left, QueryResultField right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QueryResultField left, QueryResultField right)
        {
            return !Equals(left, right);
        }
    }
}
