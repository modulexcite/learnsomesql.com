using System.Collections.Generic;
using System.Linq;
using RedGate.Publishing.Options;

namespace RedGate.Publishing.InteractiveSql
{
    public class HintGenerator
    {
        public const string WrongColumnOrder =
            "You've got the right data, but the columns seem to be in the wrong order";

        public const string TooManyColumns =
            "You seem to have selected too many columns";

        public IOption<string> GenerateHint(QueryResultTable correct, QueryResultTable submitted)
        {
            if (correct.Equals(submitted))
            {
                return Option.None<string>();
            }

            var correctColumns = Columns(correct);
            var submittedColumns = Columns(submitted);

            if (correctColumns.Count == submittedColumns.Count &&
                ContainsAllColumns(correctColumns, submittedColumns) &&
                ContainsAllColumns(submittedColumns, correctColumns)
            )
            {
                return Option.Some(WrongColumnOrder);
            }

            if (correctColumns.Count < submittedColumns.Count &&
                ContainsAllColumns(submittedColumns, correctColumns)
            )
            {
                return Option.Some(TooManyColumns);
            }

            return Option.None<string>();
        }

        private IList<IList<object>> Columns(QueryResultTable table)
        {

            return table.ColumnNames
                .Select((name, index) => (IList<object>)new List<object> {name}.Concat(table.Rows.Select(row => row.ValueAtIndex<object>(index))).ToList())
                .ToList();
        }

        private bool ContainsAllColumns(IList<IList<object>> haystack, IList<IList<object>> needles)
        {
            return needles.All(needle => haystack.Any(needle.SequenceEqual));
        }
    }
}
