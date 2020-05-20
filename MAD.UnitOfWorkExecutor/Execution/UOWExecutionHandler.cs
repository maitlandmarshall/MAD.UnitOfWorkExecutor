using MAD.UnitOfWorkExecutor.Primer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWExecutionHandler
    {
        private readonly UOWDependencyInjectionScopePrimer dependencyInjectionScopePrimer;
        private readonly UOWDependencyInjectionMethodInfoPrimer dependencyInjectionMethodInfoPrimer;

        public UOWExecutionHandler(UOWDependencyInjectionScopePrimer dependencyInjectionScopePrimer, UOWDependencyInjectionMethodInfoPrimer dependencyInjectionMethodInfoPrimer)
        {
            this.dependencyInjectionScopePrimer = dependencyInjectionScopePrimer;
            this.dependencyInjectionMethodInfoPrimer = dependencyInjectionMethodInfoPrimer;
        }

        public async Task Handle(UnitOfWork unitOfWork)
        {
            IServiceProvider uowScope = this.dependencyInjectionScopePrimer.Prime(unitOfWork);

            try
            {
                object uowInstance = uowScope.GetRequiredService(unitOfWork.MethodInfo.DeclaringType);
                IEnumerable<object> uowMethodInfoParams = this.dependencyInjectionMethodInfoPrimer.Prime(unitOfWork.MethodInfo);

                object result = unitOfWork.MethodInfo.Invoke(uowInstance, uowMethodInfoParams.ToArray());

                if (result is Task task)
                    await task;

            }
            catch (Exception)
            {

            }
            finally
            {
                (uowScope as IDisposable)?.Dispose();
            }
        }
    }
}
