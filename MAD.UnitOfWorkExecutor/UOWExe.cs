using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Primer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MAD.UnitOfWorkExecutor.Tests")]
namespace MAD.UnitOfWorkExecutor
{
    public sealed class UOWExe
    {
        public delegate void ConfigureServicesCallback(IServiceCollection serviceDescriptors);

        private IServiceProvider services;

        public async Task Start(ConfigureServicesCallback configureServices = null)
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();

            this.ConfigureServices(serviceDescriptors);

            configureServices?.Invoke(serviceDescriptors);

            ContainerBuilder autofacContainerBuilder = new ContainerBuilder();
            autofacContainerBuilder.Populate(serviceDescriptors);

            this.services = new AutofacServiceProvider(autofacContainerBuilder.Build());

            try
            {
                using var rootScope = this.services.CreateScope();

                ExecutorService unitOfWorkAttributeService = rootScope.ServiceProvider.GetRequiredService<ExecutorService>();
                await unitOfWorkAttributeService.Start();

                await Task.Delay(-1);
            }
            finally
            {
                (this.services as IDisposable)?.Dispose();
            }
        }

        private void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<ExecutorService>();
            serviceDescriptors.AddTransient<UOWFromAssemblyPrimer>();
            serviceDescriptors.AddTransient<UOWTimerPrimer>();
            serviceDescriptors.AddTransient<UOWExecutionHandler>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionScopePrimer>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionMethodInfoPrimer>();
        }
    }
}
