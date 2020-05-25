using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Schedule;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    public class UnitOfWorkExecutorBuilder
    {
        public delegate void ConfigureServicesCallback(IServiceCollection serviceDescriptors);

        private readonly IServiceCollection serviceDescriptors = new ServiceCollection();

        public UnitOfWorkExecutorBuilder ConfigureServices(ConfigureServicesCallback configureServices)
        {
            configureServices?.Invoke(serviceDescriptors);

            return this;
        }

        public UOWExe Build()
        {
            this.ConfigureServices(serviceDescriptors);

            ContainerBuilder autofacContainerBuilder = new ContainerBuilder();
            autofacContainerBuilder.Populate(serviceDescriptors);

            return new UOWExe(new AutofacServiceProvider(autofacContainerBuilder.Build()));
        }

        private void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<ExecutorService>();
            serviceDescriptors.AddTransient<UOWFromAssemblyPrimer>();
            serviceDescriptors.AddTransient<UOWScheduleFactory>();
            serviceDescriptors.AddTransient<UOWExecutionHandler>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionScopePrimer>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionMethodInfoPrimer>();
        }
    }
}
