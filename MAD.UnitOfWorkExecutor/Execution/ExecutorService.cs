using MAD.UnitOfWorkExecutor.Configuration;
using MAD.UnitOfWorkExecutor.Schedule;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal sealed class ExecutorService : BackgroundService
    {
        private readonly UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer;
        private readonly UOWExecutionHandler executionHandler;
        private readonly UOWScheduleFactory scheduleFactory;
        private readonly UOWConfigurator configurator;
        private readonly IDictionary<System.Timers.Timer, UnitOfWork> unitOfWorkTimers;

        public ExecutorService(UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer,
                               UOWExecutionHandler executionHandler,
                               UOWScheduleFactory scheduleFactory,
                               UOWConfigurator configurator)
        {
            this.unitOfWorkFromAssemblyPrimer = unitOfWorkFromAssemblyPrimer;
            this.executionHandler = executionHandler;
            this.scheduleFactory = scheduleFactory;
            this.configurator = configurator;
            this.unitOfWorkTimers = new Dictionary<System.Timers.Timer, UnitOfWork>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<UnitOfWork> unitOfWorks = this.unitOfWorkFromAssemblyPrimer.Prime(Assembly.GetEntryAssembly());

            foreach (UnitOfWork uow in unitOfWorks)
            {
                this.StartAndTrackTimerForUnitOfWork(uow);
            }
        }

        private System.Timers.Timer StartAndTrackTimerForUnitOfWork(UnitOfWork unitOfWork)
        {
            // Load the UnitOfWork metadata, such as LastDoneDateTime
            this.configurator.Load(unitOfWork);

            UOWSchedule schedule = this.scheduleFactory.Create(unitOfWork);

            System.Timers.Timer timer = new System.Timers.Timer(Math.Max(schedule.NextDue.TotalMilliseconds, 1))
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
            System.Timers.Timer timer = sender as System.Timers.Timer;
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
