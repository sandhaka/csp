using System.Linq;

namespace SchoolCalendar
{
    public static class DomainUtils
    {
        public static string DecodeClass(string id)
        {
            return id.Split('.').First().Substring(2, 1);
        }

        public static string DecodeDay(string id)
        {
            return id.Split('.')[1].Substring(1, 1);
        }

        public static string DecodeHour(string id)
        {
            return id.Split('.')[2].Substring(1, 1);
        }
    }
}