using MAD.UnitOfWorkExecutor.Configuration;
using MAD.UnitOfWorkExecutor.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWExecutionHandler
    {
        private readonly UOWExecutionScopePrimer dependencyInjectionScopePrimer;
        private readonly UOWDependencyInjectionMethodInfoPrimer dependencyInjectionMethodInfoPrimer;
        private readonly UOWConfigurator configurator;

        public UOWExecutionHandler(UOWExecutionScopePrimer dependencyInjectionScopePrimer,
                                   UOWDependencyInjectionMethodInfoPrimer dependencyInjectionMethodInfoPrimer,
                                   UOWConfigurator configurator)
        {
            this.dependencyInjectionScopePrimer = dependencyInjectionScopePrimer;
            this.dependencyInjectionMethodInfoPrimer = dependencyInjectionMethodInfoPrimer;
            this.configurator = configurator;
        }

        public async Task Handle(UnitOfWork unitOfWork)
        {
            IServiceProvider uowScope = this.dependencyInjectionScopePrimer.Prime(unitOfWork);

            try
            {
                // Create an instance of the class containing the UnitOfWorkAttribute method, the DI container will resolve any constructor paramaters
                object uowInstance = uowScope.GetRequiredService(unitOfWork.MethodInfo.DeclaringType);

                // Load metadata for the unit of work instance
                this.configurator.Load(unitOfWork, uowInstance);

                // Generate a param array to pass through to the method. This will also automatically resolve any dependencies on the method.
                IEnumerable<object> uowMethodInfoParams = this.dependencyInjectionMethodInfoPrimer.Prime(unitOfWork.MethodInfo);

                // Execute the method with the params
                object result = unitOfWork.MethodInfo.Invoke(uowInstance, uowMethodInfoParams.ToArray());

                if (result is Task task)
                    await task;
            }
            finally
            {
                (uowScope as IDisposable)?.Dispose();
            }
        }
    }
}
