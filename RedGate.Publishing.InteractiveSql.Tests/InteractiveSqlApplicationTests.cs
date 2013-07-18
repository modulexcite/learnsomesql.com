using System.Collections.Generic;
using System.Linq;
using RedGate.Publishing.InteractiveSql.Courses;
using RedGate.Publishing.Logging;
using RedGate.Publishing.Options;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests
{
    public class InteractiveSqlApplicationTests
    {
        private readonly InMemoryLogger m_Logger = new InMemoryLogger();
        private readonly InteractiveSqlApplication m_Application;
        private readonly Question m_AllLicensePlatesQuestion = new Question(
            "<p>Get the license plate of each car</p>",
            "SELECT LicensePlate FROM Cars"
        );
        private readonly Question m_AllColorsQuestion = new Question(
            "<p>Get the color of each car</p>",
            "SELECT Color FROM Cars"
        );

        public InteractiveSqlApplicationTests()
        {
            var sqlExecutor = new SqlExecutor(SqlServerConfiguration.ReadAppSettings());
            var hintGenerator = new HintGenerator();
            m_Application = new InteractiveSqlApplication(m_Logger, sqlExecutor, hintGenerator, CreateCourse());
            m_Application.Start();
        }

        [Fact]
        public void SubmittingAnswerReturnsQueryResult()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT 42");
            var result = m_Application.SubmitAnswer(answer);
            Assert.Equal("42", result.QueryResult.ExpectSuccess().Rows[0].DisplayValueAtIndex(0));
        }

        [Fact]
        public void AnswerIsCorrectIfSubmittedQueryIsTheSameAsCorrectQuery()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT LicensePlate FROM Cars");
            var result = m_Application.SubmitAnswer(answer);
            Assert.Equal(true, result.IsCorrect);
        }

        [Fact]
        public void AnswerIsCorrectIfSubmittedQueryReturnsTheSameDataIsTheSameAsCorrectQuery()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT LicensePlate FROM Cars WHERE 1=1");
            var result = m_Application.SubmitAnswer(answer);
            Assert.Equal(true, result.IsCorrect);
        }

        [Fact]
        public void AnswerIsNotCorrectIfSubmittedQueryReturnsDifferentResult()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT Color FROM Cars");
            var result = m_Application.SubmitAnswer(answer);
            Assert.Equal(false, result.IsCorrect);
        }

        [Fact]
        public void NoHintsIfResultIsCompletelyDifferentFromCorrectAnswer()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT 42");
            var result = m_Application.SubmitAnswer(answer);
            Assert.Equal(Option.None<string>(), result.Hint);
        }

        [Fact]
        public void SubmittedAnswersAreLogged()
        {
            var answer = new Answer(m_AllLicensePlatesQuestion.Identify(), "SELECT 42");
            m_Application.SubmitAnswer(answer);
            var logEvent = m_Logger.Events.Single();

            var expectedEvent = LogEvent.OfSeverity(Severity.Info)
                .Add("site", "learnsomesql.com")
                .Add("eventName", "submittedAnswer")
                .Add("question", m_AllLicensePlatesQuestion.Description)
                .Add("correctQuery", m_AllLicensePlatesQuestion.CorrectQuery)
                .Add("submittedQuery", "SELECT 42")
                .Add("isCorrect", false);

            Assert.Equal(expectedEvent.ToDictionary(), logEvent.ToDictionary());
        }

        private Course CreateCourse()
        {
            var creationSql = @"
                CREATE TABLE Cars (
	                Id INT IDENTITY NOT NULL,
                    LicensePlate VARCHAR(255) NOT NULL,
	                Color VARCHAR(255) NOT NULL
                )

                INSERT INTO Cars (LicensePlate, Color) VALUES ('X422 PWL', 'red')
                INSERT INTO Cars (LicensePlate, Color) VALUES ('B424 IFM', 'green')
            ";

            var simpleSelectsLesson = new Lesson(
                "Simple SELECTs",
                "<p>SELECTs are simple</p>",
                new List<Question> { m_AllColorsQuestion, m_AllLicensePlatesQuestion }
            );

            var lessons = new List<Lesson> {simpleSelectsLesson};
            return new Course(creationSql, lessons);
        }
    }
}
