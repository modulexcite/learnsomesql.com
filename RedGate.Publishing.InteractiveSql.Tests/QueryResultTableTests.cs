using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests
{
    public class QueryResultTableTests
    {
        [Fact]
        public void EmptyQueryResultsAreEqual()
        {
            var firstResult = new QueryResultTable(new List<string>(), new List<QueryResultRow>());
            var secondResult = new QueryResultTable(new List<string>(), new List<QueryResultRow>());
            Assert.Equal(firstResult, secondResult);
        }

        [Fact]
        public void QueryResultAreEqualIfColumnNamesAndFieldsAreTheSame()
        {
            var firstResult = new QueryResultTable(
                new List<string>{"LicensePlate", "Color"},
                new List<QueryResultRow>
                    {
                        Row("X422 PWL", "red"),
                        Row("B424 IFM", "green")
                    }
            );
            var secondResult = new QueryResultTable(
                new List<string> { "LicensePlate", "Color" },
                new List<QueryResultRow>
                    {
                        Row("X422 PWL", "red"),
                        Row("B424 IFM", "green")
                    }
            );
            Assert.Equal(firstResult, secondResult);
        }

        [Fact]
        public void QueryResultAreNotEqualIfColumnNamesDiffer()
        {
            var firstResult = new QueryResultTable(
                new List<string> { "LicensePlate", "Color" },
                new List<QueryResultRow>()
            );
            var secondResult = new QueryResultTable(
                new List<string> { "Color", "LicensePlate" },
                new List<QueryResultRow>()
            );
            Assert.NotEqual(firstResult, secondResult);
        }

        [Fact]
        public void QueryResultAreNotEqualIfRowsDiffer()
        {
            var firstResult = new QueryResultTable(
                new List<string> { "LicensePlate", "Color" },
                new List<QueryResultRow>
                    {
                        Row("X422 PWL", "red"),
                    }
            );
            var secondResult = new QueryResultTable(
                new List<string> { "Color", "LicensePlate" },
                new List<QueryResultRow>
                    {
                        Row("B424 IFM", "green")
                    }
            );
            Assert.NotEqual(firstResult, secondResult);
        }

        [Fact]
        public void ColumnNamesAreNotCaseSensitive()
        {
            var firstResult = new QueryResultTable(
                new List<string> { "color" },
                new List<QueryResultRow>()
            );
            var secondResult = new QueryResultTable(
                new List<string> { "Color" },
                new List<QueryResultRow>()
            );
            Assert.Equal(firstResult, secondResult);
        }

        private QueryResultRow Row(params string[] values)
        {
            return new QueryResultRow(values.Select(v => new QueryResultField("varchar", v)).ToList());
        }
    }
}
