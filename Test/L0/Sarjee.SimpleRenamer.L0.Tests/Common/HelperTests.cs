using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void HelperCtor_Success()
        {
            IHelper helper = new Helper();

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }
    }
}
