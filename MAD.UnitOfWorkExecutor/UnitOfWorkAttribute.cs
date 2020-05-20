using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitOfWorkAttribute : Attribute
    {
        public UnitOfWorkType WorkType { get; set; } = UnitOfWorkType.Timed;

        // Timed
        public int RunEverySeconds { get; set; } = 5;

        // Scheduled
        public DayOfWeek RunDays { get; set; }
        public int RunHour { get; set; }
        public int RunMinute { get; set; }
        public int RunSecond { get; set; }

    }
}
