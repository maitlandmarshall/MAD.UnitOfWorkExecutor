using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.UnitOfWorkExecutor.Primer
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
            var methodParams = methodInfo.GetParameters();

            if (!methodParams.Any())
                yield break;

            foreach (var mp in methodParams)
            {
                yield return this.serviceProvider.GetRequiredService(mp.ParameterType);
            }
        }
    }
}
