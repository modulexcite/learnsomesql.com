using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RedGate.Publishing.InteractiveSql.Courses;
using RedGate.Publishing.Options;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class LessonViewModel
    {
        private readonly IList<Lesson> m_Lessons;
        private readonly int m_LessonIndex;
        private readonly IDictionary<string, QueryResultTable> m_QuestionAnswers;
        private readonly string m_QuestionsJson;

        public LessonViewModel(
            IList<Lesson> lessons,
            int lessonIndex,
            IDictionary<string, QueryResultTable> questionAnswers
        )
        {
            m_Lessons = lessons;
            m_LessonIndex = lessonIndex;
            m_QuestionAnswers = questionAnswers;
            var questionsDictionaries = Lesson.Questions.Select(GenerateQuestionDictionary).ToList();
            m_QuestionsJson = JsonConvert.SerializeObject(questionsDictionaries);
        }

        private Dictionary<string, object> GenerateQuestionDictionary(Question question)
        {
            return new Dictionary<string, object>
                {
                    {"description", question.Description},
                    {"correctQuery", question.CorrectQuery},
                    {"identifier", question.Identify()},
                    {"expectedAnswer", InteractiveSqlJson.QueryResultToJson(m_QuestionAnswers[question.Identify()])}
                };
        }

        public int LessonNumber
        {
            get { return m_LessonIndex + 1; }
        }

        public Lesson Lesson
        {
            get { return m_Lessons[m_LessonIndex]; }
        }

        public string NextLessonUrl
        {
            // Convert to null for the template
            get { return NextLesson().Map(lesson => lesson.Url()).ValueOrElse(null); }
        }

        public string NextLessonTitle
        {
            // Convert to null for the template
            get { return NextLesson().Map(lesson => lesson.Title).ValueOrElse(null); }
        }

        public bool HasNextLesson
        {
            get { return NextLesson().Any(); }
        }

        public string QuestionsJson
        {
            get { return m_QuestionsJson; }
        }

        public IList<object> Lessons
        {
            get {
                return m_Lessons.Select(lesson => (object)new
                    {
                        Url = lesson.Url(),
                        Title = lesson.Title
                    }).ToList();
            }
        }

        private IOption<Lesson> NextLesson()
        {
            return Option.FromNullable(m_Lessons.ElementAtOrDefault(m_LessonIndex + 1));
        }
    }
}