using System.Text.RegularExpressions;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class Slugs
    {
        public static string Slug(string text)
        {
            var nonAlphanumericRegex = new Regex("\\W+");
            return nonAlphanumericRegex.Replace(text.ToLowerInvariant(), "-").Trim('-');
        }
    }
}
