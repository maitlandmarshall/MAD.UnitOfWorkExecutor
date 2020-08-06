using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.UnitOfWorkExecutor.Configuration;
using MAD.UnitOfWorkExecutor.Execution;
using MAD.UnitOfWorkExecutor.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MAD.UnitOfWorkExecutor
{
    public static class UOWHost
    {
        public static IHostBuilder CreateDefaultHostBuilder()
        {
            return CreateDefaultHostBuilder(null);
        }

        public static IHostBuilder CreateDefaultHostBuilder<TStartup>()
            where TStartup : UOWStartup
        {
            return CreateDefaultHostBuilder(typeof(TStartup));
        }

        private static IHostBuilder CreateDefaultHostBuilder(Type startupType)
        {
            // Define the root application builder, used for defining middleware and other application components
            UOWApplication applicationBuilder = new UOWApplication();
            AutofacServiceProviderFactory autofacServiceProviderFactory = new AutofacServiceProviderFactory();

            // Create a root serviceProvider and add the Startup type into the container
            ServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton<UOWApplication>(applicationBuilder);

            ContainerBuilder builder = autofacServiceProviderFactory.CreateBuilder(serviceDescriptors);

            if (startupType != null)
                builder.RegisterType(startupType).SingleInstance().AsSelf().As<UOWStartup>();

            IServiceProvider rootServiceProvider = autofacServiceProviderFactory.CreateServiceProvider(builder);

            // Get the root lifetime scope to be used as the appHost's parent container
            ILifetimeScope rootLifetimeScope = rootServiceProvider.GetRequiredService<ILifetimeScope>();

            IHostBuilder appHostBuilder = Host.CreateDefaultBuilder()
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacChildLifetimeScopeServiceProviderFactory(rootLifetimeScope))
                .ConfigureServices(ConfigureServices);

            if (startupType != null)
            {
                // Resolve the Startup type so the end use can inject into the constructor
                UOWStartup startup = rootServiceProvider.GetRequiredService(startupType) as UOWStartup;

                // Startup may register some dependencies
                appHostBuilder.ConfigureServices(startup.ConfigureServices);
                applicationBuilder.Startup = startup;
            }

            return appHostBuilder;
        }

        private static void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddHostedService<ExecutorService>();

            serviceDescriptors.AddTransient<UnitOfWorkFactory>();
            serviceDescriptors.AddTransient<UnitOfWorkResolver>();
            serviceDescriptors.AddTransient<UOWScheduleFactory>();
            serviceDescriptors.AddTransient<UOWExecutionHandler>();
            serviceDescriptors.AddTransient<UOWExecutionScopePrimer>();
            serviceDescriptors.AddTransient<UOWDependencyInjectionMethodInfoPrimer>();
            serviceDescriptors.AddTransient<UOWConfigurator>();
        }
    }
}
