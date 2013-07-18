using System;
using System.Collections.Generic;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests
{
    public class SqlExecutorTests
    {
        private readonly SqlServerConfiguration m_SqlServerConfiguration;

        public SqlExecutorTests()
        {
            m_SqlServerConfiguration = SqlServerConfiguration.ReadAppSettings();
        }

        [Fact]
        public void CanRunSelectStatements()
        {
            var result = ExecuteSqlOnBlankDatabase("SELECT 42");
            Assert.Equal(42, result.Rows[0].ValueAtIndex<int>(0));
        }

        [Fact]
        public void QueryResultIncludesNamesOfColumns()
        {
            var result = ExecuteSqlOnBlankDatabase("SELECT 1 as first, 2 as second");
            Assert.Equal(
                new List<ColumnName>{ColumnName.Create("first"), ColumnName.Create("second")},
                result.ColumnNames
            );
        }

        [Fact]
        public void SetUpRunsCreationSql()
        {
            var creationSql = @"
                CREATE TABLE JustTesting (
	                Id INT IDENTITY NOT NULL,
	                Answer INT NOT null
                )

                INSERT INTO JustTesting (Answer) VALUES (42)
            ";

            var application = new SqlExecutor(m_SqlServerConfiguration);
            application.SetUpDatabase(creationSql);
            var result = application.ExecuteQuery("SELECT Answer FROM JustTesting").ExpectSuccess();
            Assert.Equal(42, result.Rows[0].ValueAtIndex<int>(0));
        }

        [Fact]
        public void DisplayValueOfIntegersIsToStringOfInteger()
        {
            AssertDisplayValue("INT", "42", "42");
        }

        [Fact]
        public void DisplayValueOfStringsIsStringWithoutQuotes()
        {
            AssertDisplayValue("VARCHAR(42)", "'hello'", "hello");
        }

        [Fact]
        public void DisplayValueOfDatesIsDateInIsoFormat()
        {
            AssertDisplayValue("DATE", "'2013-03-06'", "2013-03-06");
        }

        [Fact]
        public void EmptyQueriesCauseFailedResult()
        {
            var application = new SqlExecutor(m_SqlServerConfiguration);
            application.SetUpDatabase(" ");
            var result = application.ExecuteQuery("");
            
            Assert.Equal("Query is empty", ExpectError(result));
        }

        [Fact]
        public void BadlyFormedQueriesCauseFailedResult()
        {
            var application = new SqlExecutor(m_SqlServerConfiguration);
            application.SetUpDatabase(" ");
            var result = application.ExecuteQuery("SELECTEROO");

            Assert.Equal("Could not find stored procedure 'SELECTEROO'.", ExpectError(result));
        }

        private void AssertDisplayValue(string sqlType, string sqlValue, string expectedDisplayValue)
        {
            var creationSqlTemplate = @"
                CREATE TABLE JustTesting (
	                Id INT IDENTITY NOT NULL,
	                Answer {0} NOT null
                )

                INSERT INTO JustTesting (Answer) VALUES ({1})
            ";
            var creationSql = string.Format(creationSqlTemplate, sqlType, sqlValue);

            var application = new SqlExecutor(m_SqlServerConfiguration);
            application.SetUpDatabase(creationSql);
            var result = application.ExecuteQuery("SELECT Answer FROM JustTesting").ExpectSuccess();
            Assert.Equal(expectedDisplayValue, result.Rows[0].DisplayValueAtIndex(0));
        }

        private QueryResultTable ExecuteSqlOnBlankDatabase(string sql)
        {
            var application = new SqlExecutor(m_SqlServerConfiguration);
            application.SetUpDatabase(" ");
            return application.ExecuteQuery(sql).ExpectSuccess();
        }

        private string ExpectError(IQueryResult result)
        {
            return result.Map(
                table => { throw new ApplicationException("Expected error"); },
                error => error
            );
        }
    }
}
