using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor.Execution
{
    public class UOWExecutionContext
    {
        public UnitOfWork UnitOfWork { get; internal set; }
        public IServiceProvider Services { get; internal set; }

        internal UOWExecutionContext()
        {

        }
    }
}
