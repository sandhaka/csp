namespace Csp.Csp.Model
{
    internal class Variable<T>
        where T : class
    {
        internal string Key { get; }

        internal T Value { get; set; }

        internal bool Assigned => Value != null;

        internal Variable(string key)
        {
            Key = key;
        }

        internal object ToAnonymous()
        {
            return new
            {
                Key,
                Value
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

            return (obj as Variable<T>)?.Key == Key;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}