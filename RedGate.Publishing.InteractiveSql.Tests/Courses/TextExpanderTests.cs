using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedGate.Publishing.InteractiveSql.Courses;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests.Courses
{
    public class TextExpanderTests
    {
        [Fact]
        public void HtmlElementsInTextAreUnchanged()
        {
            var expansion = Expand("<p>SELECT</p>");
            Assert.Equal("<p>SELECT</p>", expansion);
        }

        [Fact]
        public void QueryElementIsConvertedToPre()
        {
            var expansion = Expand("<query>SELECT 1</query>");
            Assert.Equal("<pre>SELECT 1</pre>", expansion);
        }

        [Fact]
        public void QueryResultsIsConvertedToQueryResultsTable()
        {
            var expansion = Expand("<query query-name='select'>SELECT 1 as one WHERE 1=2</query><query-results query-name='select' />");
            Assert.True(
                Regex.IsMatch(expansion, "<query-results-table[^>]*>\\s*</query-results-table>"),
                "Expansion was: " + expansion
            );
        }

        [Fact]
        public void QueryResultsAreIncludedInDataAttributeOfQueryResultsTable()
        {
            var expansion = Expand("<query query-name='select'>SELECT 1 as one</query><query-results query-name='select' />");
            var element = XElement.Parse(Fragment(expansion));
            var resultsTableElement = element.SingleElement("query-results-table");
            var resultsJson = JsonConvert.DeserializeObject<IDictionary<string, object>>(resultsTableElement.Attribute("data").Value);

            Assert.Equal(new JArray{"one"}, resultsJson["columnNames"]);
            var rowsJson = (JArray)resultsJson["rows"];
            Assert.Equal(new JArray{"1"}, rowsJson.Single());
        }

        private string Expand(string text)
        {
            var sqlExecutor = new SqlExecutor(SqlServerConfiguration.ReadAppSettings());
            var xmlString = string.Format("<description>{0}</description>", text);
            return new TextExpander(sqlExecutor).Expand(XElement.Parse(xmlString));
        }

        private string Fragment(string text)
        {
            return string.Format("<fragment>{0}</fragment>", text);
        }
    }
}
