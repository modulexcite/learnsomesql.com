using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public class CourseLoader
    {
        private readonly SqlExecutor m_Executor;
        private readonly TextExpander m_TextExpander;

        public CourseLoader(SqlExecutor executor, TextExpander textExpander)
        {
            m_Executor = executor;
            m_TextExpander = textExpander;
        }

        public Course Load(Stream input)
        {
            var document = XElement.Load(input);
            var creationSql = document.Elements("creation-sql").Single().Value.Trim();

            m_Executor.SetUpDatabase(creationSql);

            var lessonElements = document.Elements("lessons").Single().Elements("lesson");
            var lessons = lessonElements.Select(ReadLessonElement).ToList();

            return new Course(creationSql, lessons);
        }

        private Lesson ReadLessonElement(XElement lessonElement)
        {
            var title = lessonElement.SingleElement("title").Value.Trim();
            var description = Expand(lessonElement.SingleElement("description"));
            var questionElements = lessonElement.SingleElement("questions").Elements("question");
            var questions = questionElements.Select(ReadQuestionElement).ToList();
            return new Lesson(title, description, questions);
        }

        private Question ReadQuestionElement(XElement questionElement)
        {
            var description = Expand(questionElement.SingleElement("description"));
            var correctQuery = questionElement.SingleElement("correct-query").Value.Trim();
            return new Question(description, correctQuery);
        }

        private string Expand(XElement element)
        {
            return m_TextExpander.Expand(element);
        }
    }

    public static class XElementExtensions
    {
        public static string InnerXml(this XElement element)
        {
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        public static XElement SingleElement(this XElement element, string name)
        {
            return element.Elements(name).Single();
        }
    }
}
