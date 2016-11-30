using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
{
    [TestClass]
    public class FileWatcherTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void FileWatcherCtor_Success()
        {
            IFileWatcher fileWatcher = new FileWatcher(null, null);
            Assert.IsNotNull(fileWatcher);
        }
    }
}
