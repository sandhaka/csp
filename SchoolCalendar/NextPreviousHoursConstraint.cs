namespace SchoolCalendar
{
    public class NextPreviousHoursConstraint
    {
        public static bool Eval(string a, Teacher aVal, string b, Teacher bVal)
        {
            var aClass = DomainUtils.DecodeClass(a);
            var bClass = DomainUtils.DecodeClass(b);

            var aDay = DomainUtils.DecodeDay(a);
            var bDay = DomainUtils.DecodeDay(b);

            var aHour = int.Parse(DomainUtils.DecodeHour(a));
            var bHour = int.Parse(DomainUtils.DecodeHour(b));

            if (aClass == bClass && aDay == bDay && (aHour - 1 == bHour || aHour + 1 == bHour))
            {
                return aVal != bVal;
            }

            return true;
        }
    }
}