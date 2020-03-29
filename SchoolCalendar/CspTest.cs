using System;
using System.Collections.Generic;
using System.Linq;
using Csp.Csp;
using Csp.Resolvers.BackTrackingSearch.Parametric;
using Xunit;
using Xunit.Abstractions;

namespace SchoolCalendar
{
    /// <summary>
    /// School calendar problem combine 5 professors to fill 1 week teachings over 3 classes
    /// That's a special case because we need to track the hours spent for each teacher,
    /// we also have to try to assign them uniformly in the week and in the classes.
    /// Just a few constraints:
    /// - Teacher can't have two different classes in the same time
    /// - Teacher can't have more then one teaching hour at a time
    /// </summary>
    public class CspTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Csp<Teacher> _schoolCalendarCsp;

        public CspTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var testData = SchoolCalendarTestFactory.CreateTestData();

            var domains = testData.Select(v => new KeyValuePair<string, IEnumerable<Teacher>>(v.Key, v.Value.domains));
            var relations = testData.Select(v => new KeyValuePair<string, IEnumerable<string>>(v.Key, v.Value.relations));

            _schoolCalendarCsp = CspFactory.Create(
                new Dictionary<string, IEnumerable<Teacher>>(domains),
                new Dictionary<string, IEnumerable<string>>(relations),
                new Func<string, Teacher, string, Teacher, bool>[]
                {
                    NextPreviousHoursConstraint.Eval,
                    SimultaneouslyHoursConstraint.Eval
                }
            );
        }

        [Fact]
        public void ShouldCreateSchoolCalendarCsp()
        {
            Assert.False(_schoolCalendarCsp.Resolved);
        }

        [Fact]
        public void ShouldPropagateConsistencyWithAc3()
        {
            // Can't use arc propagation due to limit amount of the teacher hours.
            // We need a strategy with an effective assignment like backtracking search
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearch()
        {
            // Use Backtracking Search (Depth-First) to assign legal values,
            // using custom order to sort by teachers hours left
            var solved = _schoolCalendarCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<Teacher>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<Teacher>.DomainCustomOrder)
                .Resolve(() =>
                {
                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_schoolCalendarCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_schoolCalendarCsp.Resolved);
        }
    }
}