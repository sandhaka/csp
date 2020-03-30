using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleTables;
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
                    PrintPlan();
                });

            Assert.True(solved);
            Assert.True(_schoolCalendarCsp.Resolved);
        }

        private void PrintPlan()
        {
            using var writer = new StreamWriter("../../School_Calendar_Final_Planning.txt");
            var current = _schoolCalendarCsp.Status;

            foreach (var c in SchoolCalendarTestFactory.Classes)
            {
                var @class = current.Where(v => DomainUtils.DecodeClass(v.Key).Equals(c)).ToList();
                var table = new ConsoleTable("MON", "TUE", "WEN", "THU", "FRI")
                {
                    Options =
                    {
                        EnableCount = false,
                        OutputTo = writer
                    }
                };

                for (var h = 1; h <= SchoolCalendarTestFactory.NumOfDayHours; h++)
                {
                    var hour = h.ToString();
                    var th1 = @class.Single(v =>
                        DomainUtils.DecodeDay(v.Key).Equals("1") &&
                        DomainUtils.DecodeHour(v.Key).Equals(hour)).Value;
                    var th2 = @class.Single(v =>
                        DomainUtils.DecodeDay(v.Key).Equals("2") &&
                        DomainUtils.DecodeHour(v.Key).Equals(hour)).Value;
                    var th3 = @class.Single(v =>
                        DomainUtils.DecodeDay(v.Key).Equals("3") &&
                        DomainUtils.DecodeHour(v.Key).Equals(hour)).Value;
                    var th4 = @class.Single(v =>
                        DomainUtils.DecodeDay(v.Key).Equals("4") &&
                        DomainUtils.DecodeHour(v.Key).Equals(hour)).Value;
                    var th5 = @class.Single(v =>
                        DomainUtils.DecodeDay(v.Key).Equals("5") &&
                        DomainUtils.DecodeHour(v.Key).Equals(hour)).Value;

                    table.AddRow(th1.Name, th2.Name, th3.Name, th4.Name, th5.Name);
                }

                writer.WriteLine($"--- CLASS {c} ---");
                table.Write();
                writer.WriteLine();
            }
        }
    }
}