using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MAD.UnitOfWorkExecutor.Configuration
{
    internal class UOWConfigurator
    {
        private readonly IConfiguration configuration;

        public UOWConfigurator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Save(UnitOfWork unitOfWork)
        {
            throw new NotImplementedException();
        }

        public void Load(UnitOfWork unitOfWork)
        {
            string id = this.GetIdentity(unitOfWork);

            this.configuration.Bind($"{id}", unitOfWork);
        }

        private string GetIdentity(UnitOfWork unitOfWork)
        {
            return unitOfWork.MethodInfo.Name + unitOfWork.MethodInfo.DeclaringType.Name;
        }
    }
}
