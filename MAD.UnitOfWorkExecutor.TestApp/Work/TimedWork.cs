using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.TestApp.Work
{
    public class TimedWork
    {
        [UnitOfWork(RunEverySeconds = 1)]
        public async Task EveryMinute()
        {
            Console.WriteLine("YUOOO");
            await Task.Delay(5000);
        }
    }
}
