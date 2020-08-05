using MAD.UnitOfWorkExecutor.TestApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.TestApp.Work
{
    public class TimedWork
    {
        private readonly Svc1 svc1;

        public TimedWork(Svc1 svc1)
        {
            this.svc1 = svc1;
        }

        [UnitOfWork(RunAtTime = "11:51")]
        public async Task EveryMinute()
        {
            Console.WriteLine("YUOOO");
            await Task.Delay(5000);
        }
    }
}
