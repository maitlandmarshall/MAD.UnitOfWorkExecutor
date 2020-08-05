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
            TimeSpan nextDue;

            if (uow.LastRunDateTime.HasValue)
            {
                DateTime lastDone = uow.LastRunDateTime.Value;

                switch (uow.Attribute.WorkType)
                {
                    case UnitOfWorkType.Scheduled:
                        TimeSpan timeScheduled = TimeSpan.ParseExact(uow.Attribute.RunAtTime, "hh\\:mm", CultureInfo.InvariantCulture);
                        DateTime nextDueDateTime = new DateTime(lastDone.Year, lastDone.Month, lastDone.Day)
                            .AddDays(1)
                            .AddSeconds(timeScheduled.TotalSeconds);

                        nextDue =  nextDueDateTime - lastDone;

                        break;

                    case UnitOfWorkType.Timed:
                        nextDue = TimeSpan.FromSeconds(uow.Attribute.RunEverySeconds.Value);
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
