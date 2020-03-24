using Csp.Csp;
using Csp.Csp.Model;

namespace Csp.Resolvers.BackTrackingSearch
{
    internal interface ISelectUnassignedVariableStrategy<T>
        where T : class
    {
        Variable<T> Next(Csp<T> csp);
    }
}