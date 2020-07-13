using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.Configuration
{
    internal class UOWInstanceConfigurator
    {
        private readonly IConfiguration configuration;

        public UOWInstanceConfigurator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task Save(UnitOfWork unitOfWork, object unitOfWorkInstance)
        {

        }

        public Task Load(UnitOfWork unitOfWork, object unitOfWorkInstance)
        {

        }
    }
}
