using System;
using System.Collections.Generic;
using System.Linq;
using Csp.Csp;
using Csp.Csp.Model;

// Note: readability is preferred
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Csp.Resolvers
{
    /// <summary>
    /// A variable Xi is arc-consistent with respect to another variable Xj if for every value in the current domain Di
    /// there is some value in the domain Dj that satisfies the binary constraint on the arc (Xi, Xj).
    /// A network is arc-consistent if every variable is arc-consistent with every other variable.
    /// </summary>
    internal class Ac3<T> : IResolver<T>
        where T : class
    {
        private Queue<KeyValuePair<string, Variable<T>>> _queue;
        private readonly Action<Queue<KeyValuePair<string, Variable<T>>>> _arcHeuristic;

        public Ac3(
            Queue<KeyValuePair<string, Variable<T>>> queue = null,
            Action<Queue<KeyValuePair<string, Variable<T>>>> arcHeuristic = null)
        {
            _queue = queue;
            _arcHeuristic = arcHeuristic;
        }

        public bool Resolve(Csp<T> csp)
        {
            if (_queue == null)
            {
                _queue = new Queue<KeyValuePair<string, Variable<T>>>(csp.Model.FlatRelations());
            }

            _arcHeuristic?.Invoke(_queue);

            while (_queue.TryDequeue(out var pair))
            {
                var x = csp.Model.GetVariable(pair.Key);
                var y = pair.Value;
                if (Revise(csp, x, y))
                {
                    if (csp.Model.GetDomain(x.Key).IsEmpty)
                    {
                        return false;
                    }
                    _queue.Enqueue(pair);
                }
            }

            return true;
        }

        private bool Revise(Csp<T> csp, Variable<T> i, Variable<T> j)
        {
            var revised = false;

            foreach (var domainIVal in csp.Model.GetDomain(i.Key).Values)
            {
                foreach (var domainJVal in csp.Model.GetDomain(j.Key).Values)
                {
                    if (csp.Model.GetConstraints().Any(c => !c.Rule.Invoke(i.Key, domainIVal, j.Key, domainJVal)))
                    {
                        csp.Model.Prune(i.Key, domainIVal);
                        revised = true;
                    }
                }

                if (revised)
                {
                    break;
                }
            }

            return revised;
        }
    }
}