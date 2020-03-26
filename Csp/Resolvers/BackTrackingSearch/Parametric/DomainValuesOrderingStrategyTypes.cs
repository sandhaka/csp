namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public static class DomainValuesOrderingStrategyTypes<T>
        where T : class
    {
        public static readonly string UnorderedDomainValues = typeof(UnorderedDomainValues<T>).AssemblyQualifiedName;
    }
}