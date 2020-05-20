using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Primer;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor
{
    internal sealed class ExecutorService
    {
        private readonly UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer;
        private readonly UOWTimerPrimer unitOfWorkTimerPrimer;
        private readonly UOWExecutionHandler executionHandler;
        private readonly IDictionary<UnitOfWork, Timer> unitOfWorkTimers;

        public ExecutorService(UOWFromAssemblyPrimer unitOfWorkFromAssemblyPrimer, UOWTimerPrimer unitOfWorkTimerPrimer, UOWExecutionHandler executionHandler)
        {
            this.unitOfWorkFromAssemblyPrimer = unitOfWorkFromAssemblyPrimer;
            this.unitOfWorkTimerPrimer = unitOfWorkTimerPrimer;
            this.executionHandler = executionHandler;
            this.unitOfWorkTimers = new Dictionary<UnitOfWork, Timer>();
        }

        public async Task Start()
        {
            IEnumerable<UnitOfWork> unitOfWorks = this.unitOfWorkFromAssemblyPrimer.Prime(Assembly.GetEntryAssembly());

            foreach (UnitOfWork uow in unitOfWorks)
            {
                this.unitOfWorkTimers.Add(
                    key: uow,
                    value: this.unitOfWorkTimerPrimer.Prime(uow, this.OnTimerCallback)
                );
            }
        }

        private async void OnTimerCallback(object state)
        {
            if (Monitor.IsEntered(state))
                return;

            try
            {
                Monitor.Enter(state);
                await this.executionHandler.Handle(state as UnitOfWork);
            }
            finally
            {
                Monitor.Exit(state);
            }
        }
    }
}
