using System.Collections.Generic;
using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch
{
    internal interface IInferenceStrategy<T>
        where T : CspValue
    {
        bool Inference(Csp<T> csp, string varKey, T value, out IEnumerable<string> prunedDomains);
    }
}