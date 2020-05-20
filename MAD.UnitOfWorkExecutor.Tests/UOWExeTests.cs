using MAD.UnitOfWorkExecutor.Primer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MAD.UnitOfWorkExecutor.Tests
{
    

    [TestClass]
    public class UOWExeTests
    {
        [UnitOfWork]
        public class UnitOfWorkExample
        {
            [UnitOfWork]
            public void Work1()
            {

            }

        }

        [TestMethod]
        public void TestMethod1()
        {
            new UOWFromAssemblyPrimer().Prime();
        }
    }
}
