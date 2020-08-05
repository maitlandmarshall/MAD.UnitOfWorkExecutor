using MAD.UnitOfWorkExecutor.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor
{
    public interface IUOWApplication
    {
        public delegate Task MiddlewareCallback(UOWExecutionContext context, Func<Task> next);

        IList<MiddlewareCallback> Middlewares { get; }
    }
}
