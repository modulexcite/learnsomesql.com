using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public class TextExpander
    {
        private readonly SqlExecutor m_SqlExecutor;

        public TextExpander(SqlExecutor sqlExecutor)
        {
            m_SqlExecutor = sqlExecutor;
        }

        public string Expand(XElement original)
        {
            var element = new XElement(original);

            var queries = ReadQueryElements(element);

            foreach (var queryResultsElement in element.Descendants("query-results"))
            {
                var queryName = queryResultsElement.Attribute("query-name").Value;
                queryResultsElement.Name = "query-results-table";
                queryResultsElement.RemoveAttributes();
                queryResultsElement.Value = " ";
                var resultsJson = QueryResultToJson(m_SqlExecutor.ExecuteQuery(queries[queryName]));
                queryResultsElement.SetAttributeValue("data", JsonConvert.SerializeObject(resultsJson));
            }

            return element.InnerXml();
        }

        private object QueryResultToJson(IQueryResult result)
        {
            return result.Map(QueryResultTableToJson, FailureToJson);
        }

        private object QueryResultTableToJson(QueryResultTable result)
        {
            return new
                {
                    columnNames = result.ColumnNames.Select(name => name.ToString()).ToList(),
                    rows = result.Rows.Select(RowToJson).ToList()
                };
        }

        private object FailureToJson(string error)
        {
            throw new ApplicationException("Error when running query: " + error);
        }

        private object RowToJson(QueryResultRow row)
        {
            return row.Select(field => field.DisplayValue()).ToList();
        }

        private static IDictionary<string, string> ReadQueryElements(XElement element)
        {
            var result = new Dictionary<string, string>();
            foreach (var queryElement in element.Descendants("query"))
            {
                queryElement.Name = "pre";
                var queryAttribute = queryElement.Attribute("query-name");
                if (queryAttribute != null && !string.IsNullOrWhiteSpace(queryAttribute.Value))
                {
                    result[queryAttribute.Value.Trim()] = queryElement.Value.Trim();
                }
            }
            return result;
        }
    }
}
