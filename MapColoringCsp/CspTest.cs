using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Csp.Csp;
using Csp.Resolvers.BackTrackingSearch.Parametric;
using Xunit;
using Xunit.Abstractions;

namespace MapColoringCsp
{
    /// <summary>
    /// Map coloring problem as a basic test to ensure all algorithms are working effectively
    /// Color Australia regions with different colors so that the neighbors are colored by different ones
    /// </summary>
    public class CspTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Csp<ColorWrapper> _mapColoredCsp;
        private readonly IEnumerable<ColorWrapper> _colorsDomain = new ColorWrapper[] {Color.Red, Color.Green, Color.Blue};

        public CspTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mapColoredCsp = CspFactory.Create(
                new Dictionary<string, IEnumerable<ColorWrapper>>
                {
                    ["SA"] = _colorsDomain.ToList(),
                    ["WA"] = _colorsDomain.ToList(),
                    ["NT"] = _colorsDomain.ToList(),
                    ["Q"] = _colorsDomain.ToList(),
                    ["NSW"] = _colorsDomain.ToList(),
                    ["V"] = _colorsDomain.ToList(),
                    ["T"] = _colorsDomain.ToList()
                },
                new Dictionary<string, IEnumerable<string>>
                {
                    ["SA"] = new [] { "WA", "NT", "Q", "NSW", "V" },
                    ["WA"] = new [] { "SA", "NT" },
                    ["NT"] = new [] { "WA", "Q", "SA" },
                    ["Q"] = new [] { "NSW", "NT", "SA" },
                    ["NSW"] = new [] { "Q", "V", "SA" },
                    ["V"] = new [] { "NSW", "SA" }
                },
                new Func<string, ColorWrapper, string, ColorWrapper, bool>[]
                {
                    DiffValuesConstraint.Eval
                }
            );
        }
        
        [Fact]
        public void ShouldCreateAustraliaColoringMapCsp()
        {
            // Conflicts after creation must be 0
            Assert.True(_mapColoredCsp.Conflicts("SA", Color.Red) == 0);

            // Make an assigment & verify
            _mapColoredCsp.AddAssignment("T", Color.Red);
            Assert.True(_mapColoredCsp.Conflicts("SA", Color.Red) == 0);

            // Again
            _mapColoredCsp.AddAssignment("SA", Color.Blue);
            Assert.True(_mapColoredCsp.Conflicts("WA", Color.Red) == 0);
            Assert.True(_mapColoredCsp.Conflicts("WA", Color.Blue) == 1);

            _mapColoredCsp.RemoveAssignment("SA");
            Assert.True(_mapColoredCsp.Conflicts("WA", Color.Blue) == 0);
        }

        [Fact]
        public void ShouldPropagateConsistencyWithAc3()
        {
            // Start configuration
            _mapColoredCsp.AddAssignment("SA", Color.Red);
            _mapColoredCsp.ShrinkDomainToAssignment("SA");
            _mapColoredCsp.AddAssignment("WA", Color.Blue);
            _mapColoredCsp.ShrinkDomainToAssignment("WA");

            // Use Arc-Consistency propagation to reduce the legal domain values
            var solved = _mapColoredCsp
                .UseAc3AsResolver()
                .PropagateArcConsistency(() =>
                {
                    _mapColoredCsp.AutoAssignment();

                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_mapColoredCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_mapColoredCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndForwardCheckingInferenceStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _mapColoredCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<ColorWrapper>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<ColorWrapper>.UnorderedDomainValues,
                    InferenceStrategyTypes<ColorWrapper>.ForwardChecking)
                .Resolve(() =>
                {
                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_mapColoredCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_mapColoredCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndMinimumRemainingStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _mapColoredCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<ColorWrapper>.MinimumRemainingValues)
                .Resolve(() =>
                {
                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_mapColoredCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_mapColoredCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithBackTrackingSearchAndLeastConstrainingStrategy()
        {
            // Use Backtracking Search (Depth-First) to assign legal values
            var solved = _mapColoredCsp
                .UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<ColorWrapper>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<ColorWrapper>.LeastConstrainingValues)
                .Resolve(() =>
                {
                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_mapColoredCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_mapColoredCsp.Resolved);
        }

        [Fact]
        public void ShouldResolveWithHillClimbing()
        {
            // Use Hill-climbing search to find the legal combination
            var solved = _mapColoredCsp
                .UseMinConflictsResolver()
                .Resolve(() =>
                {
                    _testOutputHelper.WriteLine("==== Model: ====");
                    _testOutputHelper.WriteLine($"{_mapColoredCsp.ShowModelAsJson()}");
                    _testOutputHelper.WriteLine("================");
                });

            Assert.True(solved);
            Assert.True(_mapColoredCsp.Resolved);
        }
    }
}