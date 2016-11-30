using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
{
    [TestClass]
    public class FileMoverTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMoverCtor_Success()
        {
            IFileMover fileMover = new FileMover(null, null, null);
            Assert.IsNotNull(fileMover);
        }
    }
}
