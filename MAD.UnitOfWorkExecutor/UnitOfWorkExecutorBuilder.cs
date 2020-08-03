using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.UnitOfWorkExecutor.Configuration;
using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Schedule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;

namespace MAD.UnitOfWorkExecutor
{
    public class UnitOfWorkExecutorBuilder
    {
        public delegate void ConfigureServicesCallback(IServiceCollection serviceDescriptors);

        private readonly IServiceCollection serviceDescriptors = new ServiceCollection();

        public UnitOfWorkExecutorBuilder ConfigureServices(ConfigureServicesCallback configureServices)
        {
            configureServices?.Invoke(this.serviceDescriptors);

            return this;
        }

        public UOWExe Build()
        {
            this.ConfigureServices(this.serviceDescriptors);

            ContainerBuilder autofacContainerBuilder = new ContainerBuilder();
            autofacContainerBuilder.Populate(this.serviceDescriptors);

            return new UOWExe(new AutofacServiceProvider(autofacContainerBuilder.Build()));
        }

        private void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<ExecutorService>();
            serviceDescriptors.AddTransient<UOWFromAssemblyPrimer>();
            serviceDescriptors.AddTransient<UOWScheduleFactory>();
            serviceDescriptors.AddTransient<UOWExecutionHandler>();
            serviceDescriptors.AddTransient<UOWExecutionScopePrimer>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionMethodInfoPrimer>();
            serviceDescriptors.AddTransient<UOWInstanceConfigurator>();
            serviceDescriptors.AddTransient<UOWConfigurator>();

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfigurationRoot configuration = configurationBuilder
                .SetBasePath(Globals.BasePath)
                .AddJsonFile(
                    path: Path.GetFileName(Globals.SettingsPath),
                    optional: true,
                    reloadOnChange: true
                 )
                .Build();

            serviceDescriptors.AddSingleton<IConfiguration>(configuration);
        }
    }
}
