using System;
using System.Collections.Generic;

namespace Csp.Csp
{
    public class CspFactory
    {
        public static Csp<T> Create<T>(
            IDictionary<string, IEnumerable<T>> domains,
            IDictionary<string, IEnumerable<string>> relations,
            IEnumerable<Func<string, T, string, T, bool>> constraints
        ) where T : CspValue
        {
            // TODO: Manage here the inputs basic validations

            return new Csp<T>(domains, relations, constraints);
        }
    }
}