using MAD.UnitOfWorkExecutor.TestApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MAD.UnitOfWorkExecutor.TestApp
{
    internal class Startup : UOWStartup
    {
        public override void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<Svc1>();
        }
    }
}