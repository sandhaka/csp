namespace Csp.Csp
{
    public interface IArcConsistency<T>
        where T : CspValue
    {
        bool Ensure(Csp<T> csp);
    }
}