using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWExecutionScopePrimer
    {
        private readonly ILifetimeScope lifetimeScope;

        public UOWExecutionScopePrimer(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IServiceProvider Prime(UnitOfWork work, Action<ContainerBuilder> configureServices = null)
        {
            ILifetimeScope uowScope = this.lifetimeScope.BeginLifetimeScope(work.MethodInfo.Name, config =>
            {
                IServiceCollection serviceDescriptors = new ServiceCollection();

                this.ConfigureServices(work, serviceDescriptors);
                config.Populate(serviceDescriptors);

                configureServices?.Invoke(config);
            });

            return uowScope as IServiceProvider;
        }

        private void ConfigureServices(UnitOfWork work, IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient(work.MethodInfo.DeclaringType);
        }
    }
}
