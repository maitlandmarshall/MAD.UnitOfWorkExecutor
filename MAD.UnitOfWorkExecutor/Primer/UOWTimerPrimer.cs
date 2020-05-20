using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MAD.UnitOfWorkExecutor.Primer
{
    internal class UOWTimerPrimer
    {
        public Timer Prime(UnitOfWork work, TimerCallback timerCallback)
        {
            switch (work.Attribute.WorkType)
            {
                case UnitOfWorkType.Timed:
                    return new Timer(timerCallback, work, TimeSpan.Zero, TimeSpan.FromSeconds(work.Attribute.RunEverySeconds));

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
