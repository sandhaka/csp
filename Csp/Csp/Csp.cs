using System;
using System.Collections.Generic;
using System.Linq;
using Csp.Csp.Model;
using Csp.Resolvers;

namespace Csp.Csp
{
    public class Csp<T>
        where T : class
    {
        private CspModel<T> _model;
        private int _nAssigns = 0;
        private IArcConsistencyResolver<T> _arcConsistencyResolver;

        public int NumberOfTotalAssignments => _nAssigns;

        public Dictionary<string, T> Pruned => _model.Pruned.ToDictionary(d => d.Key, d => d.Value);

        public Csp(
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

        internal CspModel<T> Model => _model;

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

        public void AutoAssignment()
        {
            _model.AutoAssign();
        }

        public int Conflicts(string key, T value)
        {
            return _model.Conflicts(key, value);
        }

        public Csp<T> UseAc3AsResolver()
        {
            _arcConsistencyResolver = new Ac3<T>();
            return this;
        }

        public bool Resolve(Action whenResolved = null)
        {
            var resolved = _arcConsistencyResolver?.Resolve(this) ?? throw new InvalidOperationException("A resolver must be set");
            if (resolved)
            {
                whenResolved?.Invoke();
            }

            return resolved;
        }

        public string ShowModelAsJson()
        {
            return _model.ToJson();
        }
    }
}