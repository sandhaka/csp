using System.Collections.Generic;
using System.Linq;

namespace Csp.Csp.Model
{
    internal class Domain<T>
        where T : class
    {
        internal string Key { get; }
        internal List<T> Values { get; }
        internal List<T> Pruned { get; }

        internal bool IsEmpty => !Values.Any();

        internal Domain(string key, IEnumerable<T> values)
        {
            Key = key;
            Values = values.ToList();
            Pruned = new List<T>();
        }

        internal void Prune(T value)
        {
            var v = Values.Find(val => val.Equals(value));
            Pruned.Add(v);
            Values.Remove(v);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj as Domain<T>)?.Key == Key;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}