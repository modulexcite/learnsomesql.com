using System.IO;
using System.Linq;
using System.Text;
using RedGate.Publishing.InteractiveSql.Courses;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests
{
    public class CourseLoaderTests
    {
        private const string SampleCourseXml = @"<?xml version='1.0' encoding='utf-8' ?>
<course>
  <creation-sql>
    CREATE TABLE Cars (
        Id INT IDENTITY NOT NULL,
    )
  </creation-sql>
  <lessons>
    <lesson>
      <title>Simple SELECTs</title>
      <description>
        <p>SELECTs are simple.</p>
      </description>
      <questions>
        
        <question>
          <description>
            <p>Get the license plate of each car</p>
          </description>
          <correct-query>
            SELECT LicensePlate FROM Cars
          </correct-query>
        </question>

        <question>
          <description>
            <query>SELECT 1</query>
          </description>
          <correct-query>
            SELECT Color FROM Cars
          </correct-query>
        </question>
        
      </questions>
    </lesson>
  </lessons>
</course>
        ";

        [Fact]
        public void CreationSqlIsReadFromCreationSqlElement()
        {
            var course = ReadSampleCourse();
            var expectedCreationSql = 
                "CREATE TABLE Cars (\n" +
                "        Id INT IDENTITY NOT NULL,\n" +
                "    )";
            Assert.Equal(expectedCreationSql, course.CreationSql);
        }

        [Fact]
        public void LessonTitleIsReadFromTitleElement()
        {
            var course = ReadSampleCourse();
            Assert.Equal("Simple SELECTs", course.Lessons.Single().Title);
        }

        [Fact]
        public void LessonDescriptionIsReadFromDescriptionElementIncludingAnyHtml()
        {
            var course = ReadSampleCourse();
            Assert.Equal("<p>SELECTs are simple.</p>", course.Lessons.Single().Description);
        }

        [Fact]
        public void QuestionDescriptionIsReadFromDescriptionElementIncludingAnyHtml()
        {
            var course = ReadSampleCourse();
            Assert.Equal("<p>Get the license plate of each car</p>", FirstQuestion(course).Description);
        }

        [Fact]
        public void CorrectQueryIsReadFromCorrectQueryElement()
        {
            var course = ReadSampleCourse();
            Assert.Equal("SELECT LicensePlate FROM Cars", FirstQuestion(course).CorrectQuery);
        }

        [Fact]
        public void QuestionDescriptionIsExpanded()
        {
            var course = ReadSampleCourse();
            Assert.Equal("<pre>SELECT 1</pre>", Question(course, 1).Description);
        }

        private static Question FirstQuestion(Course course)
        {
            return Question(course, 0);
        }

        private static Question Question(Course course, int index)
        {
            return course.Lessons.Single().Questions[index];
        }

        private Course ReadSampleCourse()
        {
            var courseStream = new MemoryStream(Encoding.UTF8.GetBytes(SampleCourseXml));
            return ReadCourse(courseStream);
        }

        private Course ReadCourse(MemoryStream courseStream)
        {
            var sqlExecutor = new SqlExecutor(SqlServerConfiguration.ReadAppSettings());
            var textExpander = new TextExpander(sqlExecutor);
            return new CourseLoader(sqlExecutor, textExpander).Load(courseStream);
        }
    }
}
