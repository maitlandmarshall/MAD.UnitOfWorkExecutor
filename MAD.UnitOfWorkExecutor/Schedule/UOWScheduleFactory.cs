using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MAD.UnitOfWorkExecutor.Schedule
{
    internal class UOWScheduleFactory
    {
        public UOWSchedule Create(UnitOfWork uow)
        {
            DateTime now = DateTime.Now;
            TimeSpan nextDue;

            if (uow.LastRunDateTime.HasValue)
            {
                switch (uow.Attribute.WorkType)
                {
                    case UnitOfWorkType.Scheduled:
                        TimeSpan timeScheduled = TimeSpan.ParseExact(uow.Attribute.RunAtTime, "hh\\:mm", CultureInfo.InvariantCulture);
                        DateTime nextDueDateTime = new DateTime(now.Year, now.Month, now.Day).AddDays(1).AddSeconds(timeScheduled.TotalSeconds);

                        nextDue =  nextDueDateTime - now;

                        break;

                    case UnitOfWorkType.Timed:
                        nextDue = TimeSpan.FromSeconds(uow.Attribute.RunEverySeconds);
                        break;

                    default: throw new NotImplementedException();
                }
            }
            else
            {
                nextDue = TimeSpan.Zero;
            }

            return new UOWSchedule
            {
                NextDue = nextDue
            };  
        }
    }
}
