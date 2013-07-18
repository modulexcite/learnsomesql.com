using System.Collections.Generic;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public class Lesson
    {
        private readonly string m_Title;
        private readonly string m_Description;
        private readonly IList<Question> m_Questions;

        public Lesson(string title, string description, IList<Question> questions)
        {
            m_Title = title;
            m_Description = description;
            m_Questions = questions;
        }

        public string Title
        {
            get { return m_Title; }
        }

        public string Description
        {
            get { return m_Description; }
        }

        public IList<Question> Questions
        {
            get { return m_Questions; }
        }
    }
}