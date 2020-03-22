using System;
using System.Collections.Generic;
using System.Linq;

// Note: readability is preferred
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Csp.Csp.Model
{
    internal class CspModel<T>
        where T : class
    {
        private HashSet<Variable<T>> Variables { get; }
        private HashSet<Domain<T>> Domains { get; }
        private HashSet<Relations<T>> Relations { get; }
        private HashSet<Constraint<T>> Constraints { get; }

        internal CspModel(
            IEnumerable<Variable<T>> variables,
            IEnumerable<Domain<T>> domains,
            IEnumerable<Relations<T>> relations,
            IEnumerable<Constraint<T>> constraints
        )
        {
            Variables = variables.ToHashSet();
            Domains = domains.ToHashSet();
            Relations = relations.ToHashSet();
            Constraints = constraints.ToHashSet();
        }

        internal bool Validate()
        {
            // TODO: The model must be validated before used
            throw new NotImplementedException();
        }

        internal Variable<T> GetVariable(string key) => Variables.First(v => v.Key == key);
        internal Domain<T> GetDomain(string key) => Domains.First(d => d.Key == key);
        internal Relations<T> GetVariableRelations(string key) => Relations.First(r => r.Key == key);

        internal IEnumerable<KeyValuePair<string, Variable<T>>> FlatRelations() =>
            Relations.SelectMany(r =>
                r.Values.Select(v =>
                    new KeyValuePair<string, Variable<T>>(v.Key, GetVariable(r.Key))));
        internal IEnumerable<Constraint<T>> GetConstraints() => Constraints;
        internal IEnumerable<KeyValuePair<string, T>> Pruned =>
            Domains.SelectMany(d =>
                d.Pruned.Select(p =>
                    new KeyValuePair<string, T>(d.Key, p)));

        internal void Revoke(string key)
        {
            GetVariable(key).Value = null;
        }

        internal void Assign(string key, T value)
        {
            GetVariable(key).Value = value;
        }

        internal int Conflicts(string key, T value)
        {
            var c = 0;
            foreach (var neighbor in GetVariableRelations(key).Values.Where(v => v.Assigned))
            {
                foreach (var cs in Constraints)
                {
                    if (!cs.Rule.Invoke(key, value, neighbor.Key, neighbor.Value))
                    {
                        c++;
                    }
                }
            }

            return c;
        }

        internal void Prune(string key, T value)
        {
            GetDomain(key).Prune(value);
        }

        
    }
}