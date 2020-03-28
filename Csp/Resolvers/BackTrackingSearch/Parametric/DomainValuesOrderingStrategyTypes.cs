using Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public static class DomainValuesOrderingStrategyTypes<T>
        where T : CspValue
    {
        public static readonly string UnorderedDomainValues = typeof(UnorderedDomainValues<T>).AssemblyQualifiedName;
        public static readonly string LeastConstrainingValues = typeof(LeastConstrainingValues<T>).AssemblyQualifiedName;
    }
}