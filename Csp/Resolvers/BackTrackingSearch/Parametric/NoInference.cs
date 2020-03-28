using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class NoInference<T> : IInferenceStrategy<T>
        where T : CspValue
    {
        public bool Inference(Csp<T> csp, string varKey, T value)
        {
            return true;
        }
    }
}