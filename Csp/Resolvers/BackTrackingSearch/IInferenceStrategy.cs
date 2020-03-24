using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch
{
    internal interface IInferenceStrategy<T>
        where T : class
    {
        bool Inference(Csp<T> csp, string varKey, T value);
    }
}