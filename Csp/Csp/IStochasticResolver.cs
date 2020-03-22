namespace Csp.Csp
{
    public interface IStochasticResolver<T>
        where T : class
    {
        bool Resolve(Csp<T> csp);
    }
}