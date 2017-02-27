using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class RetryHelperTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RetryHelperCtor_Success()
        {
            IRetryHelper retryHelper = new RetryHelper();

            Assert.IsNotNull(retryHelper);
        }
        #endregion Constructor
    }
}
