using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Framework.TV;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Framework.TV
{
    [TestClass]
    public class GetShowDetailsTests
    {
        #region Constructor
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
        #endregion Constructor
    }
}
