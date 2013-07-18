using System.Collections.Generic;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public class Course
    {
        private readonly string m_CreationSql;
        private readonly List<Lesson> m_Lessons;

        public Course(string creationSql, List<Lesson> lessons)
        {
            m_CreationSql = creationSql;
            m_Lessons = lessons;
        }

        public string CreationSql
        {
            get { return m_CreationSql; }
        }

        public List<Lesson> Lessons
        {
            get { return m_Lessons; }
        }
    }
}