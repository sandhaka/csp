namespace Csp.Csp
{
    public interface IResolver<T>
        where T : CspValue
    {
        bool Resolve(Csp<T> csp);
    }
}