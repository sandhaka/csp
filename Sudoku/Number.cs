using System.Diagnostics.Contracts;
using Csp.Csp;

namespace Sudoku
{
    public class Number : CspValue
    {
        internal int Value { get; }

        public Number(int value)
        {
            Contract.Assert(value >= 1 && value <= 9);

            Value = value;
        }

        protected override int TypeConcernedGetHashCode() => Value.GetHashCode();

        protected override bool TypeConcernedEquals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b = (Number) obj;
            return b.Value == Value;
        }

        public override string ToString()
        {
            return Value.ToString("0");
        }
    }
}