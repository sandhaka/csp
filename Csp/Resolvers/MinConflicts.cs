using System;
using System.Linq;
using Csp.Csp;

namespace Csp.Resolvers
{
    public class MinConflicts<T> : IResolver<T>
        where T : class
    {
        private const int MaxLoop = 100000;

        public bool Resolve(Csp<T> csp)
        {
            foreach (var varKey in csp.Model.VariablesKeys)
            {
                var v = MinConflictsValue(csp, varKey);
                csp.AddAssignment(varKey, v);
            }

            for (var i = 0; i < MaxLoop; i++)
            {
                var conflicted = csp.Model.ConflictedVariables.ToList();

                if (!conflicted.Any())
                {
                    return true;
                }

                var nextVarKey = conflicted[new Random().Next(conflicted.Count)];
                var nextVal = MinConflictsValue(csp, nextVarKey);
                csp.AddAssignment(nextVarKey, nextVal);
            }

            return false;
        }

        private T MinConflictsValue(Csp<T> csp, string varKey)
        {
            var min = csp.Model.GetDomain(varKey)
                .Values
                .Select(v => (value: v, nConflicts: csp.Conflicts(varKey, v)))
                .GroupBy(i => i.nConflicts)
                .OrderBy(i => i.Key)
                .First()
                .ToList();

            return min.Count > 1 ? min[new Random().Next(min.Count)].value : min.Single().value;
        }
    }
}