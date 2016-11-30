using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Common.L0
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
