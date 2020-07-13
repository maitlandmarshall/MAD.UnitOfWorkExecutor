using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWDependencyInjectionMethodInfoPrimer
    {
        private readonly IServiceProvider serviceProvider;

        public UOWDependencyInjectionMethodInfoPrimer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEnumerable<object> Prime(MethodInfo methodInfo)
        {
            ParameterInfo[] methodParams = methodInfo.GetParameters();

            if (!methodParams.Any())
                yield break;

            foreach (ParameterInfo mp in methodParams)
            {
                yield return this.serviceProvider.GetRequiredService(mp.ParameterType);
            }
        }
    }
}
