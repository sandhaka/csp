namespace SchoolCalendar
{
    public class SimultaneouslyHoursConstraint
    {
        public static bool Eval(string a, Teacher aVal, string b, Teacher bVal)
        {
            var aDay = DomainUtils.DecodeDay(a);
            var bDay = DomainUtils.DecodeDay(b);

            var aHour = int.Parse(DomainUtils.DecodeHour(a));
            var bHour = int.Parse(DomainUtils.DecodeHour(b));

            if (aDay == bDay && aHour == bHour)
            {
                return aVal != bVal;
            }

            return true;
        }
    }
}