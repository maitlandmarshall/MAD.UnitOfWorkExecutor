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
        private readonly UnitOfWorkResolver unitOfWorkFromAssemblyPrimer;
        private readonly UOWExecutionHandler executionHandler;
        private readonly UOWScheduleFactory scheduleFactory;
        private readonly UOWConfigurator configurator;
        private readonly UOWApplication application;
        private readonly IServiceProvider serviceProvider;
        private readonly IDictionary<System.Timers.Timer, UnitOfWork> unitOfWorkTimers;

        public ExecutorService(UnitOfWorkResolver unitOfWorkFromAssemblyPrimer,
                               UOWExecutionHandler executionHandler,
                               UOWScheduleFactory scheduleFactory,
                               UOWConfigurator configurator,
                               UOWApplication application,
                               IServiceProvider serviceProvider)
        {
            this.unitOfWorkFromAssemblyPrimer = unitOfWorkFromAssemblyPrimer;
            this.executionHandler = executionHandler;
            this.scheduleFactory = scheduleFactory;
            this.configurator = configurator;
            this.application = application;
            this.serviceProvider = serviceProvider;
            this.unitOfWorkTimers = new Dictionary<System.Timers.Timer, UnitOfWork>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.application.ApplicationServices = this.serviceProvider;
            this.application.Startup?.Configure(this.application);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<UnitOfWork> unitOfWorks = this.unitOfWorkFromAssemblyPrimer.Resolve(Assembly.GetEntryAssembly());

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
