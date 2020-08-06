using MAD.UnitOfWorkExecutor.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor
{
    public class UOWApplication
    {
        public delegate Task MiddlewareCallback(UOWExecutionContext context, Func<Task> next);

        public IServiceProvider ApplicationServices { get; internal set; }
        public IList<MiddlewareCallback> Middlewares { get; } = new List<MiddlewareCallback>();

        public UOWStartup Startup { get; internal set; }

        public UOWApplication Use(UOWApplication.MiddlewareCallback middleware)
        {
            this.Middlewares.Add(middleware);

            return this;
        }
    }
}
