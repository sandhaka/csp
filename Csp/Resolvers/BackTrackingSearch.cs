using System;
using System.Collections.Generic;
using Csp.Csp;
using Csp.Csp.Model;

namespace Csp.Resolvers
{
    internal class BackTrackingSearch<T> : IBackTrackingResolver<T>
        where T : class
    {
        public bool Resolve(
            Csp<T> csp,
            Func<Variable<T>> next,
            Func<IEnumerable<T>> domainValues,
            Func<object[]> inference = null)
        {
            throw new NotImplementedException();
        }
    }
}