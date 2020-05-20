using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    internal class UnitOfWork
    {
        public readonly MethodInfo MethodInfo;
        public readonly UnitOfWorkAttribute Attribute;

        public UnitOfWork(MethodInfo ownerMethodInfo, IDictionary<string, object> attributeData)
        {
            this.MethodInfo = ownerMethodInfo;
            this.Attribute = new UnitOfWorkAttribute();

            foreach (var kvp in attributeData)
            {
                typeof(UnitOfWorkAttribute)
                    .GetProperty(kvp.Key)
                    ?.SetValue(this.Attribute, kvp.Value);
            }
        }
    }   
}
