using Autofac;
using MAD.UnitOfWorkExecutor.Configuration;
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
        private readonly UOWConfigurator configurator;
        private readonly UOWApplication application;

        public UOWExecutionHandler(UOWExecutionScopePrimer dependencyInjectionScopePrimer,
                                   UOWConfigurator configurator,
                                   UOWApplication application)
        {
            this.dependencyInjectionScopePrimer = dependencyInjectionScopePrimer;
            this.configurator = configurator;
            this.application = application;
        }

        public async Task Handle(UnitOfWork unitOfWork)
        {
            IServiceProvider uowScope = this.dependencyInjectionScopePrimer.Prime(unitOfWork);

            try
            {
                await this.ExecuteMiddlewarePipeline(unitOfWork, uowScope);
            }
            finally
            {
                (uowScope as IDisposable)?.Dispose();
            }
        }

        private async Task ExecuteMiddlewarePipeline(UnitOfWork unitOfWork, IServiceProvider uowScope, int middlewareIndex = 0)
        {
            if (middlewareIndex >= this.application.Middlewares.Count)
            {
                await this.Execute(unitOfWork, uowScope);
            }
            else
            {
                UOWApplication.MiddlewareCallback mw = this.application.Middlewares.ElementAt(middlewareIndex);

                await mw.Invoke(new UOWExecutionContext { Services = uowScope, UnitOfWork = unitOfWork }, async () =>
                 {
                     await this.ExecuteMiddlewarePipeline(unitOfWork, uowScope, middlewareIndex + 1);
                 });
            }
        }

        private async Task Execute(UnitOfWork unitOfWork, IServiceProvider uowScope)
        {
            // Create an instance of the class containing the UnitOfWorkAttribute method, the DI container will resolve any constructor paramaters
            object uowInstance = uowScope.GetRequiredService(unitOfWork.MethodInfo.DeclaringType);

            // Load metadata for the unit of work instance
            this.configurator.Load(unitOfWork, uowInstance);

            // Generate a param array to pass through to the method. This will also automatically resolve any dependencies on the method.
            UOWDependencyInjectionMethodInfoPrimer methodInjectionPrimer = uowScope.GetRequiredService<UOWDependencyInjectionMethodInfoPrimer>();
            IEnumerable<object> uowMethodInfoParams = methodInjectionPrimer.Prime(unitOfWork.MethodInfo);

            // Execute the method with the params
            object result = unitOfWork.MethodInfo.Invoke(uowInstance, uowMethodInfoParams.ToArray());
            Type resultType = result?.GetType();

            if (result is Task task)
            {
                Type taskType = task.GetType();

                await task;

                // The task can have a result
                if (taskType.IsGenericType)
                {
                    result = taskType.GetProperty(nameof(Task<object>.Result)).GetValue(task);
                    resultType = taskType.GenericTypeArguments.FirstOrDefault();
                }

                // The task has no result
                else
                {
                    result = null;
                    resultType = null;
                }
            }

            // The unit of work has finished executing successfully.
            //unitOfWork.LastRunDateTime = DateTime.Now;
            //this.configurator.Save(unitOfWork, uowInstance);

            if (result is null)
                return;

            await this.ExecuteDependentUnitsOfWork(unitOfWork, resultType, result);
        }

        private async Task ExecuteDependentUnitsOfWork(UnitOfWork parent, Type parentOutputType, object parentOutput)
        {
            if (parent.Children is null)
                return;

            foreach (UnitOfWork child in parent.Children)
            {
                IServiceProvider childScope = this.dependencyInjectionScopePrimer.Prime(child, childServices => childServices.RegisterInstance(parentOutput).As(parentOutputType));

                try
                {
                    await this.ExecuteMiddlewarePipeline(child, childScope);
                }
                finally
                {
                    (childScope as IDisposable)?.Dispose();
                }
            }
        }
    }
}
