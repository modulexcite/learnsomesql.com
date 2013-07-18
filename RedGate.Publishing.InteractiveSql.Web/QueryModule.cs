using System.IO;
using System.Linq;
using Nancy;
using Newtonsoft.Json.Linq;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class QueryModule : NancyModule
    {
        private readonly InteractiveSqlApplication m_Application;

        public QueryModule(InteractiveSqlApplication application)
        {
            m_Application = application;
            
            Post["/query"] = parameters => {
                var requestBody = RequestBody();
                return Respond(
                    requestBody["query"].Value<string>(),
                    requestBody["question-identifier"].Value<string>()
                );
            };
        }

        private JToken RequestBody()
        {
            var requestBody = new StreamReader(Request.Body).ReadToEnd();
            return JToken.Parse(requestBody);
        }

        private object Respond(string query, string questionIdentifier)
        {
            var answerResult = m_Application.SubmitAnswer(new Answer(questionIdentifier, query));
            return answerResult.QueryResult.Map(
                table => SuccessfulResponse(query, answerResult, table),
                error => FailedResponse(query, error)
            );
        }

        private object SuccessfulResponse(string query, AnswerResult answerResult, QueryResultTable table)
        {
            var resultsTable = InteractiveSqlJson.QueryResultToJson(table);
            var response = new
                {
                    query = query,
                    results = resultsTable,
                    isCorrectAnswer = answerResult.IsCorrect,
                    hint = answerResult.Hint.ValueOrElse(null)
                };
            return Json(response);
        }

        private object FailedResponse(string query, string error)
        {
            var response = new
                {
                    query = query,
                    error = error
                };
            return Json(response);
        }

        private Response Json(object body)
        {
            return Response.AsJson(body);
        }
    }

    public class InteractiveSqlJson
    {
        public static object QueryResultToJson(QueryResultTable queryResult)
        {
            var resultsTable = new
            {
                columnNames = queryResult.ColumnNames.Select(name => name.ToString()).ToList(),
                rows = queryResult.Rows.Select(RowToJson).ToList()
            };
            return resultsTable;
        }

        private static object RowToJson(QueryResultRow row)
        {
            return row.Select(field => field.DisplayValue()).ToList();
        }
    }
}