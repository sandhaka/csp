using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleTables;
using Csp.Csp;
using Csp.Resolvers.BackTrackingSearch.Parametric;
using Xunit;
using Xunit.Abstractions;

namespace Sudoku
{
    /// <summary>
    /// 9X9 Sudoku Resolvers
    /// </summary>
    public class CspTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Csp<Number> _sudokuCsp;

        public CspTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var testData = SudokuTestFactory.CreateTestData();

            var domains = testData.Select(v => new KeyValuePair<string, IEnumerable<Number>>(v.Key, v.Value.domains));
            var relations = testData.Select(v => new KeyValuePair<string, IEnumerable<string>>(v.Key, v.Value.relations));

            _sudokuCsp = CspFactory.Create(
                new Dictionary<string, IEnumerable<Number>>(domains),
                new Dictionary<string, IEnumerable<string>>(relations),
                new Func<string, Number, string, Number, bool>[]
                {
                    DiffValuesConstraint.Eval
                }
            );

            var initialConfig = SudokuTestFactory.CreateStartConfig();

            // Apply initial config
            foreach (var varValue in initialConfig)
            {
                _sudokuCsp.AddAssignment(varValue.Key, varValue.Value).ShrinkDomainToAssignment(varValue.Key);
            }
        }

        [Fact]
        public void ShouldCreateSudokuCsp()
        {
            PrintStatus("Start");
            Assert.False(_sudokuCsp.Resolved);
        }

        [Fact]
        public void ShouldPropagateConsistencyWithAc3()
        {
            // Use Arc-Consistency propagation to reduce the legal domain values
            var solved = _sudokuCsp
                .UseAc3AsResolver()
                .PropagateArcConsistency(() =>
                {
                    // Auto assign the legal values left
                    _sudokuCsp.AutoAssignment();

                    PrintStatus("AC3 End");
                    _testOutputHelper.WriteLine(Environment.NewLine);

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_sudokuCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_sudokuCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearch()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _sudokuCsp
                .UseBackTrackingSearchResolver()
                .Resolve(() =>
                {
                    PrintStatus("BTS Default End");
                    _testOutputHelper.WriteLine(Environment.NewLine);

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_sudokuCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_sudokuCsp.Resolved);
        }
        
        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndForwardCheckingInferenceStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _sudokuCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<Number>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<Number>.UnorderedDomainValues,
                    InferenceStrategyTypes<Number>.ForwardChecking)
                .Resolve(() =>
                {
                    PrintStatus("BTS Forward-Checking End");
                    _testOutputHelper.WriteLine(Environment.NewLine);

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_sudokuCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_sudokuCsp.Resolved);
        }
        
        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndMinimumRemainingStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _sudokuCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<Number>.MinimumRemainingValues)
                .Resolve(() =>
                {
                    PrintStatus("BTS MinimumRemainingValues End");
                    _testOutputHelper.WriteLine(Environment.NewLine);

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_sudokuCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_sudokuCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndLeastConstrainingStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _sudokuCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<Number>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<Number>.LeastConstrainingValues)
                .Resolve(() =>
                {
                    PrintStatus("BTS LeastConstrainingValues End");
                    _testOutputHelper.WriteLine(Environment.NewLine);

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_sudokuCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_sudokuCsp.Resolved);
        }

        private void PrintStatus(string label)
        {
            var current = _sudokuCsp.Status;

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);

            var table = new ConsoleTable
            {
                Columns = new object[] {"ColA","ColB","ColC", "*", "ColD","ColE","ColF", "*", "ColG","ColH","ColI"},
                Options =
                {
                    EnableCount = false,
                    OutputTo = writer
                }
            };

            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "9").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "8").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "7").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges("*********".ToArray().Cast<object>()).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "6").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "5").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "4").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges("*********".ToArray().Cast<object>()).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "3").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "2").Select(r => r.Value)).ToArray());
            table.AddRow(AddSquareEdges(current.Where(r => DomainUtils.Y(r.Key) == "1").Select(r => r.Value)).ToArray());

            writer.WriteLine($"==== Sudoku {label} ====");
            table.Write();
            writer.WriteLine();

            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            _testOutputHelper.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }

        private static IEnumerable<dynamic> AddSquareEdges(IEnumerable<dynamic> elems)
        {
            var nElems = elems.ToList();
            nElems.Insert(3, '*');
            nElems.Insert(7, '*');
            return nElems;
        }
    }
}