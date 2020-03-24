using Csp.Csp;
using Csp.Csp.Model;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class FirstUnassignedVariable<T> : ISelectUnassignedVariableStrategy<T>
        where T : class
    {
        public Variable<T> Next(Csp<T> csp)
        {
            return csp.Model.GetFirstVariable(v => !v.Assigned);
        }
    }
}