using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
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
