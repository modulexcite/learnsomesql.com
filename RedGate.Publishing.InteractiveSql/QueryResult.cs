using System;
using System.Collections.Generic;

namespace RedGate.Publishing.InteractiveSql
{
    public static class QueryResult
    {
        public static IQueryResult Success(IList<string> columnNames, IList<QueryResultRow> rows)
        {
            return new QuerySuccess(new QueryResultTable(columnNames, rows));
        }

        public static IQueryResult Failure(string error)
        {
            return new QueryFailure(error);
        }
    }

    public interface IQueryResult
    {
        T Map<T>(Func<QueryResultTable, T> success, Func<string, T> failure);
        QueryResultTable ExpectSuccess();
    }

    public class QuerySuccess : IQueryResult
    {
        private readonly QueryResultTable m_Results;

        internal QuerySuccess(QueryResultTable results)
        {
            m_Results = results;
        }

        public T Map<T>(Func<QueryResultTable, T> success, Func<string, T> failure)
        {
            return success(m_Results);
        }

        public QueryResultTable ExpectSuccess()
        {
            return m_Results;
        }
    }

    public class QueryFailure : IQueryResult
    {
        private readonly string m_Error;

        internal QueryFailure(string error)
        {
            m_Error = error;
        }

        public T Map<T>(Func<QueryResultTable, T> success, Func<string, T> failure)
        {
            return failure(m_Error);
        }

        public QueryResultTable ExpectSuccess()
        {
            throw new ApplicationException("Expected success, but had error: " + m_Error);
        }
    }
}
