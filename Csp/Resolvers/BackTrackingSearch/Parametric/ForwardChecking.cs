using System.Linq;
using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public class ForwardChecking<T> : IInferenceStrategy<T>
        where T : CspValue
    {
        public bool Inference(Csp<T> csp, string varKey, T value)
        {
            foreach (var var in csp.Model.VariableRelations(varKey).Values.Where(v => !v.Assigned))
            {
                foreach (var val in csp.Model.GetDomain(var.Key).Values.ToList())
                {
                    if (csp.Model.GetConstraints().Any(c => !c.Rule.Invoke(varKey, value, var.Key, val)))
                    {
                        csp.Model.Prune(var.Key, val);
                    }
                }

                if (!csp.Model.GetDomain(var.Key).Values.Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}