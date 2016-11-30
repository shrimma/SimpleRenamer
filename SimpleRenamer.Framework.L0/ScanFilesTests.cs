﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.L0;

namespace SimpleRenamer.Framework.Core.L0
{
    [TestClass]
    public class ScanFilesTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Core)]
        public void ScanFilesCtor_Success()
        {
            IScanFiles scanFiles = new ScanFiles(null, null, null, null, null, null);
            Assert.IsNotNull(scanFiles);
        }
    }
}
