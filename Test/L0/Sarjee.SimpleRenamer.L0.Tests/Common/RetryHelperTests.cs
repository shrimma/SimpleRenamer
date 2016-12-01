using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class RetryHelperTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RetryHelperCtor_Success()
        {
            IRetryHelper retryHelper = new RetryHelper();

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }
    }
}
