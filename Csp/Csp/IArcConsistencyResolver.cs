using System;
using System.Collections.Generic;
using Csp.Csp.Model;

namespace Csp.Csp
{
    internal interface IArcConsistencyResolver<T>
        where T : class
    {
        bool Resolve(
            Csp<T> csp,
            Queue<KeyValuePair<string, Variable<T>>> queue = null,
            Action<Queue<KeyValuePair<string, Variable<T>>>> heuristic = null);
    }
}