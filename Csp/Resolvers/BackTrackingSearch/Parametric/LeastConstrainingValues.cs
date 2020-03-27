using System.Collections.Generic;
using System.Linq;
using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public class LeastConstrainingValues<T> : IDomainValuesOrderingStrategy<T>
        where T : class
    {
        public IEnumerable<T> GetDomainValues(Csp<T> csp, string key)
        {
            return csp.Model.GetDomain(key).Values
                .Select(v => (v, nConflicts: csp.Conflicts(key, v)))
                .OrderByDescending(v => v.nConflicts)
                .Select(v => v.v);
        }
    }
}