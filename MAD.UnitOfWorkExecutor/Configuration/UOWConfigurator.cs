using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MAD.UnitOfWorkExecutor.Configuration
{
    internal class UOWConfigurator
    {
        private static readonly object syncToken = new object();

        private JObject GetSettingsFileJson(string path)
        {
            JObject settingsFileJson;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                settingsFileJson = JObject.Parse(json);
            }
            else
            {
                settingsFileJson = new JObject();
            }

            return settingsFileJson;
        }

        public void Save(UnitOfWork unitOfWork, object unitOfWorkInstance)
        {
            /* File will look like this: {
             *   "[UnitOfWorkMethodName]" : {
             *   
             *   }
             * }
             */

            string fullFilePath = this.GetFilePath(unitOfWork);

            lock (syncToken)
            {
                JObject settingsFile = this.GetSettingsFileJson(fullFilePath);

                // Overwrite the node representing this Unit of Work
                JObject unitOfWorkJson = settingsFile[this.GetIdentity(unitOfWork)] as JObject;

                if (unitOfWorkJson is null)
                {
                    unitOfWorkJson = new JObject();
                }

                // Combine the UnitOfWork and it's instance into one json object
                JObject unitOfWorkSerialized = JObject.FromObject(unitOfWork);
                unitOfWorkSerialized.Merge(JObject.FromObject(unitOfWorkInstance));

                // Merge the existing json with the new json
                unitOfWorkJson.Merge(unitOfWorkSerialized);

                // Overwrite the old node
                settingsFile[this.GetIdentity(unitOfWork)] = unitOfWorkJson;

                // Write back to the settings file
                if (!Directory.Exists(Globals.SettingsDirectory))
                    Directory.CreateDirectory(Globals.SettingsDirectory);

                string fileJson = settingsFile.ToString();
                File.WriteAllText(fullFilePath, fileJson);
            }
        }

        public void Load(UnitOfWork unitOfWork, object unitOfWorkInstance = null)
        {
            string fullFilePath = this.GetFilePath(unitOfWork);

            lock (syncToken)
            {
                JObject settingsFile = this.GetSettingsFileJson(fullFilePath);
                JToken unitOfWorkJson = settingsFile[this.GetIdentity(unitOfWork)];

                if (unitOfWorkJson == null)
                    return;

                // Restore the LastDoneDateTime
                unitOfWork.LastRunDateTime = unitOfWorkJson.Value<DateTime?>(nameof(UnitOfWork.LastRunDateTime));

                if (unitOfWorkInstance != null)
                {
                    // Populate the values ontop of the unitOfWorkInstance
                    JsonConvert.PopulateObject(unitOfWorkJson.ToString(), unitOfWorkInstance);
                }
            }
        }

        private string GetIdentity(UnitOfWork unitOfWork)
        {
            return unitOfWork.MethodInfo.Name;
        }

        private string GetFilePath(UnitOfWork unitOfWork)
        {
            return Path.Combine(Globals.SettingsDirectory, $"{unitOfWork.MethodInfo.DeclaringType.Name}.json");
        }
    }
}
