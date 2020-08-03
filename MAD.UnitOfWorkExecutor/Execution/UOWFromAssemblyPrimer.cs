using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWFromAssemblyPrimer
    {
        private readonly UnitOfWorkFactory unitOfWorkFactory;

        public UOWFromAssemblyPrimer(UnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<UnitOfWork> Prime(Assembly assembly)
        {
            // Scan the entry assembly for any methods which use the UnitOfWorkAttribute
            IEnumerable<Type> exportedTypes = assembly.ExportedTypes;

            foreach (Type t in exportedTypes)
            {
                IEnumerable<UnitOfWork> result = t
                    .GetMethods()
                    .Where(y => y.GetCustomAttribute<UnitOfWorkAttribute>() != null)
                    .Select(y => this.unitOfWorkFactory.Create(y));

                foreach (UnitOfWork uom in result)
                {
                    yield return uom;
                }
            }
        }
    }
}
