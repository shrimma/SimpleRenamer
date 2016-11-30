using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Common.L0
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
