namespace Csp.Csp
{
    public interface IResolver<T>
        where T : class
    {
        bool Resolve(Csp<T> csp);
    }
}