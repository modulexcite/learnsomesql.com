using RedGate.Publishing.Options;

namespace RedGate.Publishing.InteractiveSql
{
    public class AnswerResult
    {
        private readonly IQueryResult m_QueryResult;
        private readonly bool m_IsCorrect;
        private readonly IOption<string> m_Hint;

        public AnswerResult(IQueryResult queryResult, bool isCorrect, IOption<string> hint)
        {
            m_QueryResult = queryResult;
            m_IsCorrect = isCorrect;
            m_Hint = hint;
        }

        public IQueryResult QueryResult
        {
            get { return m_QueryResult; }
        }

        public bool IsCorrect
        {
            get { return m_IsCorrect; }
        }

        public IOption<string> Hint
        {
            get { return m_Hint; }
        }
    }
}