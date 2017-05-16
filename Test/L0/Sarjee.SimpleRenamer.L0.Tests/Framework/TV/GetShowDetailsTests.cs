using FluentAssertions;
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
        private static MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
        private Mock<ILogger> mockLogger;
        private Mock<ITvdbManager> mockTvdbManager;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = mockRepository.Create<ILogger>();
            mockTvdbManager = mockRepository.Create<ITvdbManager>();
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void GetShowDetailsCtor_NullArguments_ThrowsArgumentNullException()
        {
            Action action1 = () => new GetShowDetails(null, null);
            Action action2 = () => new GetShowDetails(mockLogger.Object, null);

            action1.ShouldThrow<ArgumentNullException>();
            action2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.TV)]
        public void GetShowDetailsCtor_Success()
        {
            IGetShowDetails getShowDetails = null;
            Action action1 = () => getShowDetails = new GetShowDetails(mockLogger.Object, mockTvdbManager.Object);

            action1.ShouldNotThrow();
            getShowDetails.Should().NotBeNull();
        }
        #endregion Constructor
    }
}
