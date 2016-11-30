using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
{
    [TestClass]
    public class FileMatcherTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileMatcherCtor_Success()
        {
            IFileMatcher fileMatcher = new FileMatcher(null, null);
            Assert.IsNotNull(fileMatcher);
        }
    }
}
