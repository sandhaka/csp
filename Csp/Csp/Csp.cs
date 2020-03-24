using System;
using System.Collections.Generic;
using System.Linq;
using Csp.Csp.Model;
using Csp.Resolvers;
using Csp.Resolvers.BackTrackingSearch;
using Csp.Resolvers.BackTrackingSearch.Parametric;

namespace Csp.Csp
{
    public class Csp<T>
        where T : class
    {
        private readonly CspModel<T> _model;
        private IResolver<T> _resolver;
        private int _nAssigns;

        internal Csp(
            IDictionary<string, IEnumerable<T>> domains,
            IDictionary<string, IEnumerable<string>> relations,
            IEnumerable<Func<string, T, string, T, bool>> constraints
        )
        {
            var variables = domains.Select(d => new Variable<T>(d.Key)).ToList();

            _model = new CspModel<T>(
                variables,
                domains.Select(d => new Domain<T>(d.Key, d.Value)).ToList(),
                relations.Select(d => new Relations<T>(d.Key, variables.Where(v => d.Value.Contains(v.Key)))).ToList(),
                constraints.Select(d => new Constraint<T>(d)).ToList()
            );
        }

        #region [ Model wrappers ]

        internal CspModel<T> Model => _model;

        public bool Resolved => _model.Resolved;

        public Csp<T> RemoveAssignment(string variableKey)
        {
            _model.Revoke(variableKey);
            return this;
        }

        public Csp<T> AddAssignment(string variableKey, T value)
        {
            _model.Assign(variableKey, value);
            _nAssigns++;
            return this;
        }

        public void AutoAssignment() => _model.AutoAssign();

        public int Conflicts(string key, T value) => _model.Conflicts(key, value);

        public string ShowModelAsJson() => _model.ToJson();

        #endregion

        #region [ Api ]

        public int NumberOfTotalAssignments => _nAssigns;

        public Dictionary<string, T> Pruned => _model.Pruned.ToDictionary(d => d.Key, d => d.Value);

        public Csp<T> UseAc3AsResolver()
        {
            _resolver = new Ac3<T>();
            return this;
        }

        public Csp<T> UseBackTrackingSearchResolver(Config config)
        {
            IInferenceStrategy<T> inferenceStrategy;
            IDomainValuesOrderingStrategy<T> domainValuesOrderingStrategy;
            ISelectUnassignedVariableStrategy<T> selectUnassignedVariableStrategy;

            switch (config)
            {
                case Config.Default:
                {
                    inferenceStrategy = new NoInference<T>();
                    domainValuesOrderingStrategy = new UnorderedDomainValues<T>();
                    selectUnassignedVariableStrategy = new FirstUnassignedVariable<T>();
                    break;
                }
                case Config.AppliedHeuristic:
                {
                    throw new NotImplementedException();
                }
                default:
                {
                    inferenceStrategy = new NoInference<T>();
                    domainValuesOrderingStrategy = new UnorderedDomainValues<T>();
                    selectUnassignedVariableStrategy = new FirstUnassignedVariable<T>();
                    break;
                }
            }

            _resolver = new BackTrackingSearch<T>(
                selectUnassignedVariableStrategy,
                domainValuesOrderingStrategy,
                inferenceStrategy);

            return this;
        }

        public bool Resolve(Action whenResolved = null)
        {
            var resolved = _resolver?.Resolve(this) ?? throw new InvalidOperationException("A resolver must be set");

            if (resolved)
            {
                whenResolved?.Invoke();
            }

            return resolved;
        }

        #endregion
    }
}