using System;
using System.Collections.Generic;
using System.Linq;
using Csp.Csp.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

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

            Validate();
        }

        internal Variable<T> GetVariable(string key) => Variables.First(v => v.Key == key);
        internal Variable<T> GetFirstVariable(Func<Variable<T>, bool> predicate) => Variables.FirstOrDefault(predicate);
        internal Domain<T> GetDomain(string key) => Domains.First(d => d.Key == key);
        internal IEnumerable<Constraint<T>> GetConstraints() => Constraints;

        internal IEnumerable<string> VariablesKeys => Variables.Select(v => v.Key);
        internal IEnumerable<string> ConflictedVariables => Variables
                .Where(v => Conflicts(v.Key, v.Value) > 0)
                .Select(v => v.Key);
        internal Relations<T> VariableRelations(string key) =>
            Relations.FirstOrDefault(r =>
                r.Key == key) ?? new Relations<T>(key, new List<Variable<T>>());
        internal IEnumerable<KeyValuePair<string, Variable<T>>> FlatRelations() =>
            Relations.SelectMany(r =>
                r.Values.Select(v =>
                    new KeyValuePair<string, Variable<T>>(v.Key, GetVariable(r.Key))));
        internal IEnumerable<KeyValuePair<string, T>> PrunedDomainValues =>
            Domains.SelectMany(d =>
                d.Pruned.Select(p =>
                    new KeyValuePair<string, T>(d.Key, p)));
        internal bool IsAllAssigned => Variables.All(v => v.Assigned);
        internal bool Resolved => IsAllAssigned && Variables.All(v => Conflicts(v.Key, v.Value) == 0);

        internal void Revoke(string key)
        {
            GetVariable(key).Value = null;
        }

        internal void Assign(string key, T value)
        {
            GetVariable(key).Value = value;
        }

        internal void AutoAssign()
        {
            foreach (var variable in Variables)
            {
                Assign(variable.Key, GetDomain(variable.Key).Values.First());
            }
        }

        internal virtual int Conflicts(string key, T value)
        {
            var c = 0;
            foreach (var neighbor in VariableRelations(key).Values.Where(v => v.Assigned))
            {
                foreach (var cs in Constraints)
                {
                    if (!cs.Rule.Invoke(key, value, neighbor.Key, neighbor.Value))
                    {
                        c++;
                        break;
                    }
                }
            }

            return c;
        }

        internal void Prune(string key, T value)
        {
            GetDomain(key).Prune(value);
        }

        internal void Suppose(string key, T value)
        {
            GetDomain(key).Suppose(value);
        }

        internal void RestoreGuess(string key)
        {
            GetDomain(key).RestoreGuess(key);
        }

        internal string ToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Variables = Variables.Select(v => v.ToAnonymous()).ToList(),
                Domains = Domains.Select(d => d.ToAnonymous()).ToList(),
                Relations = Relations.Select(r => r.ToAnonymous()).ToList()
            }, Formatting.Indented);
        }

        private void Validate()
        {
            var errors = new List<ValidationFailure>();

            foreach (var result in Variables.Select(variable => new VariableValidator<T>().Validate(variable)).Where(result => !result.IsValid))
            {
                errors.AddRange(result.Errors.ToList());
            }

            foreach (var result in Domains.Select(domain => new DomainValidator<T>().Validate(domain)).Where(result => !result.IsValid))
            {
                errors.AddRange(result.Errors.ToList());
            }

            foreach (var result in Relations.Select(relation => new RelationsValidator<T>().Validate(relation)).Where(result => !result.IsValid))
            {
                errors.AddRange(result.Errors.ToList());
            }

            foreach (var result in Constraints.Select(constraint => new ConstraintValidator<T>().Validate(constraint)).Where(result => !result.IsValid))
            {
                errors.AddRange(result.Errors.ToList());
            }

            if (errors.Any())
            {
                throw new ArgumentException(errors.Select(e => e.ErrorMessage).Aggregate((a, b) => $"{a},{b}"));
            }

            if (Relations.Any(r => Variables.All(v => v.Key != r.Key)))
            {
                throw new ArgumentException("Relationships/Variables mismatch");
            }

            if (Relations.Any(r => r.Values.Any(rv => Variables.All(v => v.Key != rv.Key))))
            {
                throw new ArgumentException("Relationships/Variables mismatch");
            }

            if (Domains.Any(d => !d.Values.Any()))
            {
                throw new ArgumentException("Domain start size cannot be zero");
            }
        }
    }
}