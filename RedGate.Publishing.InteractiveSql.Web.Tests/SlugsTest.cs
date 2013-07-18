using Xunit;

namespace RedGate.Publishing.InteractiveSql.Web.Tests
{
    public class SlugsTest
    {
        [Fact]
        public void SlugifyConvertsCharactersToLowerCase()
        {
            Assert.Equal("sql", Slugs.Slug("SQL"));
        }

        [Fact]
        public void SlugifyConvertsNonAlphanumericCharactersToHyphens()
        {
            Assert.Equal("sql-selects", Slugs.Slug("sql selects"));
        }

        [Fact]
        public void RunsOfNonAlphanumericCharactersAreConvertedToSingleHyphen()
        {
            Assert.Equal("select-where", Slugs.Slug("select & where"));
        }

        [Fact]
        public void LeadingNonAlphanumericCharactersAreStripped()
        {
            Assert.Equal("select", Slugs.Slug("+select"));
        }

        [Fact]
        public void TrailingNonAlphanumericCharactersAreStripped()
        {
            Assert.Equal("select", Slugs.Slug("select!"));
        }
    }
}
