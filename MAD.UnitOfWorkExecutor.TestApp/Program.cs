using System;

namespace MAD.UnitOfWorkExecutor.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new UnitOfWorkExecutorBuilder().Build().Run().Wait();
        }
    }
}
