using System.Reflection;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public static class SqlSelectCourse
    {
        public static Course Load(SqlExecutor executor)
        {
            var courseStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("RedGate.Publishing.InteractiveSql.Courses.SqlSelectCourse.xml");
            return new CourseLoader(executor, new TextExpander(executor)).Load(courseStream);
        }
    }
}
