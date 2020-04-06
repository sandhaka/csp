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
        where T : CspValue
    {
        private readonly CspModel<T> _model;
        private IResolver<T> _resolver;
        private IArcConsistency<T> _arcConsistency;
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

        public void ShrinkDomainToAssignment(string key) => _model.ShrinkDomainToAssignment(key);

        public void SortBeforeAutoAssignment() => _model.SortAndAutoAssign();

        public void RandomAssignment() => _model.RandomAssign();

        public int Conflicts(string key, T value) => _model.Conflicts(key, value);

        public string ShowModelAsJson() => _model.ToJson();

        #endregion

        #region [ Api ]

        public int NumberOfTotalAssignments => _nAssigns;

        public Dictionary<string, T> Pruned => _model.PrunedDomainValues.ToDictionary(d => d.Key, d => d.Value);

        public Dictionary<string, T> Status => _model.Status();

        public Csp<T> UseAc3AsResolver()
        {
            _arcConsistency = new Ac3<T>();
            return this;
        }

        public Csp<T> UseBackTrackingSearchResolver(
            string selectStrategyType = "",
            string domainOrderingStrategyType = "",
            string infStrategyType = "")
        {
            var infType = Type.GetType(infStrategyType) ?? typeof(NoInference<T>);
            var domainOrdType = Type.GetType(domainOrderingStrategyType) ?? typeof(UnorderedDomainValues<T>);
            var selectType = Type.GetType(selectStrategyType) ?? typeof(FirstUnassignedVariable<T>);

            _resolver = new BackTrackingSearch<T>(
                (ISelectUnassignedVariableStrategy<T>) Activator.CreateInstance(selectType),
                (IDomainValuesOrderingStrategy<T>) Activator.CreateInstance(domainOrdType),
                (IInferenceStrategy<T>) Activator.CreateInstance(infType));

            return this;
        }

        public Csp<T> UseMinConflictsResolver()
        {
            _resolver = new MinConflicts<T>();
            return this;
        }

        public bool Resolve(Action whenResolved = null)
        {
            var resolved = _resolver?.Resolve(this) ??
                           throw new InvalidOperationException("A resolver must be set");

            if (resolved)
            {
                whenResolved?.Invoke();
            }

            return resolved;
        }

        public bool PropagateArcConsistency(Action whenEnsured = null)
        {
            var propagated = _arcConsistency?.Propagate(this) ??
                             throw new InvalidOperationException("An arc consistency method must be set");

            if (propagated)
            {
                whenEnsured?.Invoke();
            }

            return propagated;
        }

        #endregion
    }
}