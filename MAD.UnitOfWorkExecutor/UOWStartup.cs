using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    public abstract class UOWStartup
    {
        public virtual void Configure(UOWApplication application)
        {

        }

        public virtual void ConfigureServices(IServiceCollection serviceDescriptors)
        {

        }
    }
}
