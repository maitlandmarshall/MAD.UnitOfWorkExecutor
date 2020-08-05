using MAD.UnitOfWorkExecutor.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor
{
    public class UOWApplicationBuilder : IUOWApplication
    {
        public IList<IUOWApplication.MiddlewareCallback> Middlewares { get; } = new List<IUOWApplication.MiddlewareCallback>();

        public UOWApplicationBuilder Use(IUOWApplication.MiddlewareCallback middleware)
        {
            this.Middlewares.Add(middleware);

            return this;
        }
    }
}
