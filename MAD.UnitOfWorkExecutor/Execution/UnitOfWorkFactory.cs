using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MAD.UnitOfWorkExecutor.Execution
{
    internal class UnitOfWorkFactory
    {
        public UnitOfWork Create(MethodInfo y)
        {
            UnitOfWorkAttribute uowAttribute = y.GetCustomAttribute<UnitOfWorkAttribute>();
            IDictionary<string, object> uowAttributeData = y.GetCustomAttributesData()
                .Where(z => z.AttributeType == typeof(UnitOfWorkAttribute))
                    .Select(y => y.NamedArguments
                        .ToDictionary(
                            keySelector: keySelector => keySelector.MemberName,
                            elementSelector: elementSelector => elementSelector.TypedValue.Value
                            )
                        )
                    .FirstOrDefault();

            if (uowAttribute is null)
                throw new ArgumentException($"Method must be decorated with [{nameof(UnitOfWorkAttribute)}].");

            return new UnitOfWork(
                ownerMethodInfo: y,
                attributeData: uowAttributeData);
        }
    }
}
