using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Schedule;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace MAD.UnitOfWorkExecutor
{
    internal sealed class ExecutorService
    {
        private readonly UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer;
        private readonly UOWExecutionHandler executionHandler;
        private readonly UOWScheduleFactory scheduleFactory;
        private readonly IDictionary<Timer, UnitOfWork> unitOfWorkTimers;

        public ExecutorService(UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer, UOWExecutionHandler executionHandler, UOWScheduleFactory scheduleFactory)
        {
            this.unitOfWorkFromAssemblyPrimer = unitOfWorkFromAssemblyPrimer;
            this.executionHandler = executionHandler;
            this.scheduleFactory = scheduleFactory;
            this.unitOfWorkTimers = new Dictionary<Timer, UnitOfWork>();
        }

        public async Task Start()
        {
            IEnumerable<UnitOfWork> unitOfWorks = this.unitOfWorkFromAssemblyPrimer.Prime(Assembly.GetEntryAssembly());

            foreach (UnitOfWork uow in unitOfWorks)
            {
                this.StartAndTrackTimerForUnitOfWork(uow);
            }
        }

        private Timer StartAndTrackTimerForUnitOfWork(UnitOfWork unitOfWork)
        {
            UOWSchedule schedule = this.scheduleFactory.Create(unitOfWork);

            Timer timer = new Timer(Math.Max(schedule.NextDue.TotalMilliseconds, 1))
            {
                AutoReset = false
            };
            timer.Elapsed += this.Timer_Elapsed;

            this.unitOfWorkTimers.Add(timer, unitOfWork);

            timer.Start();

            return timer;
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer timer = sender as Timer;
            UnitOfWork unitOfWork = this.unitOfWorkTimers[timer];

            timer.Dispose();
            this.unitOfWorkTimers.Remove(timer);

            try
            {
                await this.executionHandler.Handle(unitOfWork);
            }
            finally
            {
                unitOfWork.LastRunDateTime = DateTime.Now;
                this.StartAndTrackTimerForUnitOfWork(unitOfWork);
            }
        }
    }
}
