using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MAD.UnitOfWorkExecutor.Configuration
{
    public class ConfigurationStore
    {
        private static readonly object syncToken = new object();

        public void Save(object objectToSave, string path)
        {
            lock (syncToken)
            {
                
            }
        }
    }
}
