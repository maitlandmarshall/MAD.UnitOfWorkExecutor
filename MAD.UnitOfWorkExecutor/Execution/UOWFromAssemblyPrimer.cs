using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UOWFromAssemblyPrimer
    {
        public IEnumerable<UnitOfWork> Prime(Assembly assembly)
        {
            // Scan the entry assembly for any types or methods which use the UnitOfWorkAttribute
            IEnumerable<Type> exportedTypes = assembly.ExportedTypes;

            foreach (Type t in exportedTypes)
            {
                var methodsWithAttribute = t
                    .GetMethods()
                    .Where(y => y.GetCustomAttribute<UnitOfWorkAttribute>() != null)
                    .Select(y => new
                    {
                        MethodInfo = y,
                        UnitOfWorkAttributeData = y.GetCustomAttributesData()
                            .Where(z => z.AttributeType == typeof(UnitOfWorkAttribute))
                            .Select(y => y.NamedArguments
                                .ToDictionary(
                                    keySelector: keySelector => keySelector.MemberName,
                                    elementSelector: elementSelector => elementSelector.TypedValue.Value
                                    )
                                )
                            .FirstOrDefault()
                    });

                foreach (var m in methodsWithAttribute)
                {
                    yield return new UnitOfWork(m.MethodInfo, m.UnitOfWorkAttributeData);
                }
            }
        }
    }
}
