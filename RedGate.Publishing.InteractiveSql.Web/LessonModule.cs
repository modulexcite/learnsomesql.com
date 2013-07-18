using System.Linq;
using Nancy;
using RedGate.Publishing.InteractiveSql.Courses;
using RedGate.Publishing.Options;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class LessonModule : NancyModule
    {
        private readonly InteractiveSqlApplication m_Application;
        private readonly SqlExecutor m_SqlExecutor;

        public LessonModule(InteractiveSqlApplication application, SqlExecutor sqlExecutor)
        {
            m_Application = application;
            m_SqlExecutor = sqlExecutor;
            var firstLesson = m_Application.Course.Lessons.First();
            Get["/"] = parameters => Response.AsRedirect(firstLesson.Url());
            Get["/lesson/{lessonName}"] = parameters => RenderLesson(parameters.lessonName);
        }

        private object RenderLesson(string lessonName)
        {
            return FindLessonByName(lessonName)
                .Map(RenderLessonWithIndex)
                .ValueOrElse(404);
        }

        private object RenderLessonWithIndex(int lessonIndex)
        {
            var lesson = m_Application.Course.Lessons[lessonIndex];
            var questionAnswers = lesson.Questions.ToDictionary(
                question => question.Identify(),
                Answer
            );
            var viewModel = new LessonViewModel(
                m_Application.Course.Lessons,
                lessonIndex,
                questionAnswers
            );
            return View["lesson", viewModel];
        }

        private IOption<int> FindLessonByName(string lessonName)
        {
            var index = m_Application.Course.Lessons.FindIndex(lesson => lesson.Name() == lessonName);
            return index == -1 ? Option.None<int>() : Option.Some(index);
        }

        private QueryResultTable Answer(Question question)
        {
            return m_SqlExecutor
                .ExecuteQuery(question.CorrectQuery)
                .ExpectSuccess();
        }
    }
}
