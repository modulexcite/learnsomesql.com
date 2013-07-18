using System.Collections.Generic;
using System.Linq;
using RedGate.Publishing.Options;
using Xunit;

namespace RedGate.Publishing.InteractiveSql.Tests
{
    public class HintGeneratorTests
    {
        private static readonly HintGenerator HintGenerator = new HintGenerator();

        [Fact]
        public void CompletelyWrongAnswerGivesNoHint()
        {
            var correct = Table(ColumnNames("first"), Row(1));
            var submitted = Table(ColumnNames(), Row());
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.None<string>(), hint);
        }

        [Fact]
        public void NoHintIfAnswerIsCorrect()
        {
            var correct = Table(ColumnNames("first"), Row(1));
            var hint = HintGenerator.GenerateHint(correct, correct);
            Assert.Equal(Option.None<string>(), hint);
        }

        [Fact]
        public void HintIfAnswerHasColumnsInWrongOrder()
        {
            var correct = Table(ColumnNames("month", "year"), Row(1, 2012), Row(4, 2013));
            var submitted = Table(ColumnNames("year", "month"), Row(2012, 1), Row(2013, 4));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.Some(HintGenerator.WrongColumnOrder), hint);
        }

        [Fact]
        public void ChecksForAllCorrectColumnsWhenCheckingForColumnsInWrongOrder()
        {
            var correct = Table(ColumnNames("month", "month"), Row(1, 2012), Row(4, 2013));
            var submitted = Table(ColumnNames("year", "month"), Row(4, 2013), Row(1, 2012));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.None<string>(), hint);
        }

        [Fact]
        public void ChecksForAllSubmittedColumnsWhenCheckingForColumnsInWrongOrder()
        {
            var correct = Table(ColumnNames("month", "year"), Row(1, 2012), Row(4, 2013));
            var submitted = Table(ColumnNames("month", "month"), Row(4, 2013), Row(1, 2012));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.None<string>(), hint);
        }

        [Fact]
        public void ChecksValuesOfColumnsWhenCheckingForColumnsInWrongOrder()
        {
            var correct = Table(ColumnNames("month", "year"), Row(1, 2012), Row(4, 2013));
            var submitted = Table(ColumnNames("year", "month"), Row(3, 2013), Row(1, 2012));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.None<string>(), hint);
        }

        [Fact]
        public void HintIfAnswerHasTooManyColumns()
        {
            var correct = Table(ColumnNames("month"), Row(1), Row(4));
            var submitted = Table(ColumnNames("year", "month"), Row(2012, 1), Row(2013, 4));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.Some(HintGenerator.TooManyColumns), hint);
        }

        [Fact]
        public void ChecksValuesOfColumnsWhenCheckingForTooManyColumns()
        {
            var correct = Table(ColumnNames("month"), Row(0), Row(4));
            var submitted = Table(ColumnNames("year", "month"), Row(2012, 1), Row(2013, 4));
            var hint = HintGenerator.GenerateHint(correct, submitted);
            Assert.Equal(Option.None<string>(), hint);
        }

        private QueryResultTable Table(IList<string> columnNames, params QueryResultRow[] rows)
        {
            return new QueryResultTable(columnNames, rows);
        }

        private IList<string> ColumnNames(params string[] names)
        {
            return names;
        }

        private QueryResultRow Row(params int[] values)
        {
            var sqlValues = values
                .Select(value => new QueryResultField("int", value))
                .ToList();
            return new QueryResultRow(sqlValues);
        }
    }
}
