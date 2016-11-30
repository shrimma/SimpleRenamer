using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
{
    [TestClass]
    public class ActionMatchedFilesTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ActionMatchedFilesCtor_Success()
        {
            IActionMatchedFiles actionMatchedFiles = new ActionMatchedFiles(null, null, null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }
    }
}
