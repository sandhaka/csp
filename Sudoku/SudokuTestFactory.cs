using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public static class SudokuTestFactory
    {
        public static string N = DomainUtils.N.ToString("0");
        private static readonly IEnumerable<int> NumOfRowsAndCols = "123456789".ToCharArray()
            .Select(c => int.Parse(c.ToString()))
            .ToArray();

        public static Dictionary<string, (IEnumerable<Number> domains, IEnumerable<string> relations)> CreateTestData()
        {
            var numbers = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}.Select(n => new Number(n));

            var variablesId = CreateVariablesId().ToList();

            var relations = new Dictionary<string, List<string>>();

            foreach (var key in variablesId)
            {
                var xKey = DomainUtils.X(key);
                var yKey = DomainUtils.Y(key);
                var keySquare = DomainUtils.Square(key);

                var related = variablesId.Where(v =>
                        v != key &&
                        (DomainUtils.X(v) == xKey ||
                        DomainUtils.Y(v) == yKey ||
                        DomainUtils.Square(v) == keySquare))
                    .ToList();

                relations.Add(key, related);
            }

            var data = new Dictionary<string, (IEnumerable<Number> domains, IEnumerable<string> relations)>(
                variablesId.Select(v => new KeyValuePair<string, (IEnumerable<Number> domains, IEnumerable<string> relations)>(
                    v, (numbers.ToList(), relations.Where(r => r.Key == v).SelectMany(r => r.Value))
                ))
            );

            return data;
        }

        public static IDictionary<string, Number> CreateStartConfig()
        {
            return new Dictionary<string, Number>
            {
                ["3.1"] = new Number(5),
                ["5.1"] = new Number(1),
                ["7.1"] = new Number(3),
                ["1.2"] = new Number(8),
                ["4.2"] = new Number(2),
                ["6.2"] = new Number(3),
                ["9.2"] = new Number(9),
                ["3.3"] = new Number(2),
                ["4.3"] = new Number(6),
                ["6.3"] = new Number(9),
                ["7.3"] = new Number(5),
                ["3.4"] = new Number(6),
                ["4.4"] = new Number(7),
                ["6.4"] = new Number(8),
                ["7.4"] = new Number(2),
                ["1.5"] = new Number(7),
                ["9.5"] = new Number(8),
                ["3.6"] = new Number(8),
                ["4.6"] = new Number(1),
                ["6.6"] = new Number(2),
                ["7.6"] = new Number(9),
                ["3.7"] = new Number(1),
                ["4.7"] = new Number(8),
                ["6.7"] = new Number(6),
                ["7.7"] = new Number(4),
                ["1.8"] = new Number(9),
                ["4.8"] = new Number(3),
                ["6.8"] = new Number(5),
                ["9.8"] = new Number(1),
                ["3.9"] = new Number(3),
                ["5.9"] = new Number(2),
                ["7.9"] = new Number(6)
            };
        }

        private static IEnumerable<string> CreateVariablesId()
        {
            return (from x in NumOfRowsAndCols from y in NumOfRowsAndCols select $"{x}.{y}").ToList();
        }
    }
}