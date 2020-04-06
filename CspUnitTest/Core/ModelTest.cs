using System.Collections.Generic;
using System.Linq;
using Csp.Csp.Model;
using Xunit;

namespace CspUnitTest.Core
{
    public class ModelTest
    {
        private CspModel<DummyCspValue> _model;

        private static bool Eval(string a, DummyCspValue aVal, string b, DummyCspValue bVal)
        {
            return aVal != bVal;
        }

        public ModelTest()
        {
            var dummyDomain = new[] { "R", "B", "G" };

            var proto = new Dictionary<string, IEnumerable<DummyCspValue>>
            {
                ["SA"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["WA"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["NT"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["Q"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["NSW"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["V"] = dummyDomain.Select(c => new DummyCspValue(c)),
                ["T"] = dummyDomain.Select(c => new DummyCspValue(c))
            };
            var relationsProto = new Dictionary<string, IEnumerable<string>>
            {
                ["SA"] = new[] {"WA", "NT", "Q", "NSW", "V"},
                ["WA"] = new[] {"SA", "NT"},
                ["NT"] = new[] {"WA", "Q", "SA"},
                ["Q"] = new[] {"NSW", "NT", "SA"},
                ["NSW"] = new[] {"Q", "V", "SA"},
                ["V"] = new[] {"NSW", "SA"}
            };

            var variables = proto.Select(d => new Variable<DummyCspValue>(d.Key)).ToList();

            _model = new CspModel<DummyCspValue>(
                variables,
                proto.Select(v => new Domain<DummyCspValue>(v.Key, v.Value)).ToList(),
                relationsProto.Select(d => new Relations<DummyCspValue>(d.Key, variables.Where(v => d.Value.Contains(v.Key)))).ToList(),
                new[]
                {
                    new Constraint<DummyCspValue>(Eval)
                }
            );
        }

        [Fact]
        public void ShouldCalculateConflict()
        {
            // Setup
            _model.Assign("NSW", new DummyCspValue("R"));
            _model.Assign("Q", new DummyCspValue("R"));

            // Act
            var nConflicts = _model.Conflicts("SA", new DummyCspValue("R"));

            // Verify
            Assert.Equal(2, nConflicts);
            Assert.Equal(3, _model.GetDomain("NSW").Values.Count);
            Assert.Equal(3, _model.GetDomain("Q").Values.Count);
            Assert.Equal(3, _model.GetDomain("SA").Values.Count);
        }

        [Fact]
        public void ShouldSortAndAssign()
        {
            // Act
            _model.SortAndAutoAssign();

            // Verify
            Assert.True(_model.GetDomain("SA").Values.First().C.ToCharArray().First().Equals('B'));
            Assert.True(_model.GetDomain("SA").Values.Last().C.ToCharArray().First().Equals('R'));
            Assert.True(_model.IsAllAssigned);
        }
    }
}