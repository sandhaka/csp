using System.Collections.Generic;
using System.Linq;

namespace SchoolCalendar
{
    public static class SchoolCalendarTestFactory
    {
        public static readonly string[] Classes = { "A", "B", "C" };
        public const int NumOfWeekDays = 5;
        public const int NumOfDayHours = 6;

        public static Dictionary<string, (IEnumerable<Teacher> domains, IEnumerable<string> relations)> CreateTestData()
        {
            var teachers = CreateTeachers().ToList();

            var variablesId = CreateVariablesId().ToList();

            var relations = new Dictionary<string, List<string>>();

            // Create simultaneously hours relations
            for (var h = 1; h <= NumOfDayHours; h++)
            {
                var hour = h.ToString();
                var neighbors = variablesId.Where(v => DomainUtils.DecodeHour(v).Equals(hour)).ToList();

                foreach (var nv in neighbors)
                {
                    relations.Add(nv, neighbors.Where(n =>
                            n != nv &&    // Exclude itself
                            DomainUtils.DecodeDay(nv).Equals(DomainUtils.DecodeDay(n))) // Same day
                        .ToList());
                }
            }

            // Create previous/next hours relations
            for (var d = 1; d <= NumOfWeekDays; d++)
            {
                var day = d.ToString();
                var neighbors = variablesId.Where(v => DomainUtils.DecodeDay(v).Equals(day)).ToList();

                foreach (var @class in Classes)
                {
                    var neighborsOfCurrClass = neighbors.Where(v => DomainUtils.DecodeClass(v).Equals(@class)).ToList();

                    foreach (var nv in neighborsOfCurrClass)
                    {
                        var currentHour = int.Parse(DomainUtils.DecodeHour(nv));
                        relations.First(v => v.Key == nv).Value.AddRange(neighborsOfCurrClass.Where(n =>
                        {
                            var h = int.Parse(DomainUtils.DecodeHour(n));
                            return h + 1 == currentHour || h - 1 == currentHour;
                        }));
                    }
                }
            }

            var data = new Dictionary<string, (IEnumerable<Teacher> domains, IEnumerable<string> relations)>(
                variablesId.Select(v => new KeyValuePair<string, (IEnumerable<Teacher> domains, IEnumerable<string> relations)>(
                    v, (teachers, relations.Where(r => r.Key == v).SelectMany(r => r.Value))
                ))
            );

            return data;
        }

        private static IEnumerable<Teacher> CreateTeachers()
        {
            // 100h total capacity
            return new List<Teacher>
            {
                new Teacher("Peppino Spazzeguti",16, "Math"),
                new Teacher("Gino Brambilla", 16, "Literature"),
                new Teacher("Filiberto Grotti", 16, "History"),
                new Teacher("Carlo Ciampi", 16, "English"),
                new Teacher("Elio Insonne", 16, "Science"),
                new Teacher("Adriana Volpe", 16, "Chemistry")
            };
        }

        private static IEnumerable<string> CreateVariablesId()
        {
            var hours = new List<string>();
            foreach (var @class in Classes)
            {
                for (var d = 1; d <= NumOfWeekDays; d++)
                {
                    for (var h = 1; h <= NumOfDayHours; h++)
                    {
                        hours.Add(
                            $"CL{@class}." + // Class
                            $"D{d}." + // Day
                            $"H{h}" // Hour
                        );
                    }
                }
            }

            return hours;
        }
    }
}