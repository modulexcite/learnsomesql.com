using System.Collections.Generic;
using System.Linq;
using RedGate.Publishing.InteractiveSql.Courses;
using RedGate.Publishing.Logging;
using RedGate.Publishing.Options;

namespace RedGate.Publishing.InteractiveSql
{
    public class InteractiveSqlApplication
    {
        private readonly ILogger m_Logger;
        private readonly SqlExecutor m_SqlExecutor;
        private readonly HintGenerator m_HintGenerator;
        private readonly Course m_Course;
        private readonly IDictionary<string, Question> m_Questions;

        public InteractiveSqlApplication(ILogger logger, SqlExecutor sqlExecutor, HintGenerator hintGenerator, Course course)
        {
            m_Logger = logger;
            m_SqlExecutor = sqlExecutor;
            m_HintGenerator = hintGenerator;
            m_Course = course;

            m_Questions = course.Lessons
                .SelectMany(lesson => lesson.Questions)
                .ToDictionary(question => question.Identify());
        }

        public Course Course
        {
            get { return m_Course; }
        }

        public AnswerResult SubmitAnswer(Answer answer)
        {
            var queryResult = m_SqlExecutor.ExecuteQuery(answer.Query);
            var question = m_Questions[answer.QuestionIdentifier];

            var correctResult = m_SqlExecutor.ExecuteQuery(question.CorrectQuery).ExpectSuccess();
            var isCorrect = queryResult.Map(
                correctResult.Equals,
                error => false
            );

            var hint = queryResult.Map(
                submittedResult => m_HintGenerator.GenerateHint(correctResult, submittedResult),
                error => Option.None<string>()
            );
            var answerResult = new AnswerResult(queryResult, isCorrect, hint);

            m_Logger.Log(LogEventForAnswer(question, answer, answerResult));

            return answerResult;
        }

        private LogEvent LogEventForAnswer(Question question, Answer answer, AnswerResult answerResult)
        {
            return LogEvent
                .OfSeverity(Severity.Info)
                .Add("site", "learnsomesql.com")
                .Add("eventName", "submittedAnswer")
                .Add("question", question.Description)
                .Add("correctQuery", question.CorrectQuery)
                .Add("submittedQuery", answer.Query)
                .Add("isCorrect", answerResult.IsCorrect);
        }

        public void Start()
        {
            m_SqlExecutor.SetUpDatabase(m_Course.CreationSql);
        }


    }
}
