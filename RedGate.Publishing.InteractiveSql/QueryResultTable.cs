using System;
using System.Collections.Generic;
using System.Linq;

namespace RedGate.Publishing.InteractiveSql
{
    public class QueryResultTable : IEquatable<QueryResultTable>
    {
        private readonly IList<ColumnName> m_ColumnNames;
        private readonly IList<QueryResultRow> m_Rows;

        public QueryResultTable(IList<string> columnNames, IList<QueryResultRow> rows)
        {
            m_ColumnNames = columnNames.Select(ColumnName.Create).ToList();
            m_Rows = rows;
        }

        public IList<ColumnName> ColumnNames
        {
            get { return m_ColumnNames; }
        }

        public IList<QueryResultRow> Rows
        {
            get { return m_Rows; }
        }

        public bool Equals(QueryResultTable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return m_ColumnNames.SequenceEqual(other.m_ColumnNames)
                && m_Rows.SequenceEqual(other.m_Rows);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryResultTable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_ColumnNames.GetHashCode() * 397) ^ m_Rows.GetHashCode();
            }
        }

        public static bool operator ==(QueryResultTable left, QueryResultTable right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QueryResultTable left, QueryResultTable right)
        {
            return !Equals(left, right);
        }
    }
}