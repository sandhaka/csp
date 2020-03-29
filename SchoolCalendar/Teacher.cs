using System;
using Csp.Csp;

namespace SchoolCalendar
{
    public class Teacher : CspValue, IComparable<Teacher>
    {
        public string Name { get; }
        public int TotalHours { get; }
        public int SpentHours { get; private set; }
        public int HoursLeft => TotalHours - SpentHours;
        public string Subject { get; }

        public Teacher(string name, int totalHours, string subject)
        {
            Name = name;
            TotalHours = totalHours;
            Subject = subject;
            SpentHours = 0;
        }

        protected override int TypeConcernedGetHashCode()
        {
            return Name.GetHashCode();
        }

        protected override bool TypeConcernedEquals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b2 = (Teacher)obj;
            return Name == b2.Name;
        }

        public override void AssignmentCallback()
        {
            SpentHours++;
            HoursCheck();
        }

        public override void RevokeCallback()
        {
            SpentHours--;
            HoursCheck();
        }

        private void HoursCheck()
        {
            if (SpentHours > TotalHours || SpentHours < 0)
            {
                throw new InvalidOperationException($"{Name}, Business rule violation: 0 <= Spent hours <= Total hours");
            }
        }

        public int CompareTo(Teacher other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return SpentHours.CompareTo(other.SpentHours);
        }
    }
}