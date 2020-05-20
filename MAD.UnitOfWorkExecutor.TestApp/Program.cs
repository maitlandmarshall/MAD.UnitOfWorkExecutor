using System;

namespace MAD.UnitOfWorkExecutor.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new UOWExe().Start().Wait();
        }
    }
}
