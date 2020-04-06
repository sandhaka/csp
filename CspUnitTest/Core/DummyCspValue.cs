using System;
using Csp.Csp;

namespace CspUnitTest.Core
{
    public class DummyCspValue : CspValue, IComparable<DummyCspValue>
    {
        public string C { get; }

        public DummyCspValue(string c)
        {
            C = c;
        }

        protected override int TypeConcernedGetHashCode()
        {
            return C.GetHashCode();
        }

        protected override bool TypeConcernedEquals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b2 = (DummyCspValue)obj;
            return C == b2.C;
        }

        public int CompareTo(DummyCspValue other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(C, other.C, StringComparison.Ordinal);
        }

        public override void AssignmentCallback()
        {

        }

        public override void RevokeCallback()
        {

        }
    }
}