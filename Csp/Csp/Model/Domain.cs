using System.Collections.Generic;
using System.Linq;

namespace Csp.Csp.Model
{
    internal class Domain<T>
        where T : CspValue
    {
        internal string Key { get; }
        internal List<T> Values { get; }
        internal List<T> Pruned { get; }
        internal List<T> RemovedByGuess { get; private set; }

        internal bool IsEmpty => !Values.Any();

        internal Domain(string key, IEnumerable<T> values)
        {
            Key = key;
            Values = values.ToList();
            Pruned = new List<T>();
            RemovedByGuess = new List<T>();
        }

        internal void Prune(T value)
        {
            var v = Values.Find(val => val.Equals(value));
            Pruned.Add(v);
            Values.Remove(v);
        }

        internal void Suppose(T value)
        {
            RemovedByGuess = Values.Where(v => v != value).ToList();
            Values.RemoveAll(v => v != value);
        }

        internal void RestoreGuess(string key)
        {
            Values.AddRange(RemovedByGuess);
            RemovedByGuess.Clear();
        }

        internal object ToAnonymous()
        {
            return new
            {
                Key,
                Values = Values.ToList()
            };
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