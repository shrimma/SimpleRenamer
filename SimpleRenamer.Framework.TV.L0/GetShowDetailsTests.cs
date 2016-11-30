using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.Framework.TV;
using SimpleRenamer.L0;
using System;

namespace SimpleRenamer.Framework.L0
{
    [TestClass]
    public class GetShowDetailsTests
    {
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetShowDetailsCtor_NullLogger_ThrowsArgumentNullException()
        {
            IGetShowDetails getShowDetails = new GetShowDetails(null, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetShowDetailsCtor_NullTvdbManager_ThrowsArgumentNullException()
        {
            ILogger logger = new Mock<ILogger>().Object;
            IGetShowDetails getShowDetails = new GetShowDetails(logger, null);

            //we shouldnt get here so throw if we do
            Assert.IsTrue(false);
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void GetShowDetailsCtor_Success()
        {
            ILogger logger = new Mock<ILogger>().Object;
            ITvdbManager tvdbManager = new Mock<ITvdbManager>().Object;
            IGetShowDetails getShowDetails = new GetShowDetails(logger, tvdbManager);

            Assert.IsNotNull(getShowDetails);
        }
    }
}
