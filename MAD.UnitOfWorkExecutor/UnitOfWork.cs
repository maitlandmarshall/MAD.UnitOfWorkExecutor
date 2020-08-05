using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MAD.UnitOfWorkExecutor
{
    public class UnitOfWork
    {
        public UnitOfWork(MethodInfo ownerMethodInfo, IDictionary<string, object> attributeData)
        {
            this.MethodInfo = ownerMethodInfo;
            this.Attribute = new UnitOfWorkAttribute();

            foreach (KeyValuePair<string, object> kvp in attributeData)
            {
                typeof(UnitOfWorkAttribute)
                    .GetProperty(kvp.Key)
                    ?.SetValue(this.Attribute, kvp.Value);
            }
        }

        internal IEnumerable<UnitOfWork> Children { get; set; }

        [JsonIgnore]
        public MethodInfo MethodInfo { get; }

        [JsonIgnore]
        public UnitOfWorkAttribute Attribute { get; }

        public DateTime? LastRunDateTime { get; set; }
    }
}
