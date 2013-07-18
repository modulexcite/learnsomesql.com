using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RedGate.Publishing.InteractiveSql
{
    public class QueryResultRow : IEquatable<QueryResultRow>, IEnumerable<QueryResultField>
    {
        private readonly IList<QueryResultField> m_Values;

        public QueryResultRow(IList<QueryResultField> values)
        {
            m_Values = values;
        }

        public T ValueAtIndex<T>(int index)
        {
            return (T) m_Values[index].Value;
        }

        public string DisplayValueAtIndex(int index)
        {
            return m_Values[index].DisplayValue();
        }

        public IEnumerator<QueryResultField> GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(QueryResultRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_Values.SequenceEqual(other.m_Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryResultRow) obj);
        }

        public override int GetHashCode()
        {
            return (m_Values != null ? m_Values.GetHashCode() : 0);
        }

        public static bool operator ==(QueryResultRow left, QueryResultRow right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QueryResultRow left, QueryResultRow right)
        {
            return !Equals(left, right);
        }
    }
}