using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
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
