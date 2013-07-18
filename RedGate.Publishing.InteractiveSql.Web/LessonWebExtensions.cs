using RedGate.Publishing.InteractiveSql.Courses;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public static class LessonWebExtensions
    {
        public static string Url(this Lesson lesson)
        {
            return string.Format("/lesson/{0}", lesson.Name());
        }

        public static string Name(this Lesson lesson)
        {
            return Slugs.Slug(lesson.Title.Replace("*", "star"));
        }
    }
}
