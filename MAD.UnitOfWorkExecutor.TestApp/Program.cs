using Microsoft.Extensions.Hosting;
using System;

namespace MAD.UnitOfWorkExecutor.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            UOWHost.CreateDefaultHostBuilder<Startup>().Build().Run();
        }
    }
}
