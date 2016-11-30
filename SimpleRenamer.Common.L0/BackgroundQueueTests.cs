using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Common.L0
{
    /// <summary>
    /// Summary description for BackgroundQueueTests
    /// </summary>
    [TestClass]
    public class BackgroundQueueTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void BackgroundQueueCtor_Success()
        {
            IBackgroundQueue backgroundQueue = new BackgroundQueue();

            //we shouldnt get here so throw if we do
            Assert.IsNotNull(backgroundQueue);
        }
    }
}
