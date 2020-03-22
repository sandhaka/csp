using System;
using System.Collections.Generic;
using Csp.Csp.Model;

namespace Csp.Csp
{
    internal interface IBackTrackingResolver<T>
        where T : class
    {
        bool Resolve(
            Csp<T> csp,
            Func<Variable<T>> next,
            Func<IEnumerable<T>> domainValues,
            Func<object[]> inference = null);
    }
}