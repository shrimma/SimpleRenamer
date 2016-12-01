using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Core;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.Core
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
