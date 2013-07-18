namespace RedGate.Publishing.InteractiveSql
{
    public class Answer
    {
        private readonly string m_QuestionIdentifier;
        private readonly string m_Query;

        public Answer(string questionIdentifier, string query)
        {
            m_QuestionIdentifier = questionIdentifier;
            m_Query = query;
        }

        public string QuestionIdentifier
        {
            get { return m_QuestionIdentifier; }
        }

        public string Query
        {
            get { return m_Query; }
        }
    }
}