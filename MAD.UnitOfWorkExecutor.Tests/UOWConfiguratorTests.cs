using MAD.UnitOfWorkExecutor.Configuration;
using MAD.UnitOfWorkExecutor.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor.Tests
{
    [TestClass]
    public class UOWConfiguratorTests
    {
        private class ConfigTestUOWInstance
        {
            public string Property1 { get; set; } = "heeleleo";
            public string Property2 { get; set; } = "DINGOOOZ";

            [UnitOfWork(RunAtTime = "22:20")]
            public string Method1()
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public void Save_SimpleAndPrimitiveProperties_JsonOk()
        {
            UnitOfWork uow = new UnitOfWorkFactory().Create(typeof(ConfigTestUOWInstance).GetMethod(nameof(ConfigTestUOWInstance.Method1)));
            uow.LastRunDateTime = DateTime.Now;

            var config = new UOWConfigurator();

            config.Save(uow, new ConfigTestUOWInstance());
        }

        [TestMethod]
        public void Load_SimpleAndPrimitiveProperties_Ok()
        {
            UnitOfWork uow = new UnitOfWorkFactory().Create(typeof(ConfigTestUOWInstance).GetMethod(nameof(ConfigTestUOWInstance.Method1)));
            uow.LastRunDateTime = DateTime.Now;

            var config = new UOWConfigurator();

            var instance = new ConfigTestUOWInstance
            {
                Property1 = null,
                Property2 = null
            };
            config.Load(uow, instance);
        }
    }
}
