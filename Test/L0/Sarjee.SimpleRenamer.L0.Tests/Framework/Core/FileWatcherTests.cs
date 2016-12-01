using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
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
