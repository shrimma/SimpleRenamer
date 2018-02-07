using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.L0.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class HelperTests
    {
        private static IRestClient _restClient;
        private static JsonSerializerSettings _jsonSerializerSettings;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _restClient = new RestClient();
            _jsonSerializerSettings = new JsonSerializerSettings();
        }

        private IHelper GetHelper()
        {
            IHelper helper = new Helper();
            helper.Should().NotBeNull();
            return helper;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void HelperCtor_Success()
        {
            IHelper helper = null;
            Action action1 = () => helper = GetHelper();

            action1.ShouldNotThrow();
            helper.Should().NotBeNull();
        }
        #endregion Constructor

        #region IsFileExtensionValid
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_Empty_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            bool result = true;
            Action action1 = () => result = helper.IsFileExtensionValid(string.Empty);

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_JustFullStop_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            bool result = true;
            Action action1 = () => result = helper.IsFileExtensionValid(".");

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_MissingFullStop_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            bool result = true;
            Action action1 = () => result = helper.IsFileExtensionValid("fil");

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_InvalidChar_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            bool result = true;
            Action action1 = () => result = helper.IsFileExtensionValid(".fil*");

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_Valid_ReturnsTrue()
        {
            IHelper helper = GetHelper();

            bool result = false;
            Action action1 = () => result = helper.IsFileExtensionValid(".fil");

            action1.ShouldNotThrow();
            result.Should().BeTrue();
        }
        #endregion IsFileExtensionValid

        #region AreListsEqual
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_EmptyStringLists_ReturnsTrue()
        {
            IHelper helper = GetHelper();

            //setup lists
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();

            bool result = false;
            Action action1 = () => result = helper.AreListsEqual(list1, list2);

            action1.ShouldNotThrow();
            result.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedEqualStringLists_ReturnsTrue()
        {
            IHelper helper = GetHelper();

            //setup lists
            List<string> list1 = new List<string>
            {
                "123456789"
            };
            List<string> list2 = new List<string>
            {
                "123456789",
                "1"
            };
            list2.Remove("1");

            bool result = false;
            Action action1 = () => result = helper.AreListsEqual(list1, list2);

            action1.ShouldNotThrow();
            result.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedDifferentCountStringLists_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            //setup lists
            List<string> list1 = new List<string>
            {
                "123456789",
                "123456789"
            };
            List<string> list2 = new List<string>
            {
                "123456789"
            };
            bool result = true;
            Action action1 = () => result = helper.AreListsEqual(list1, list2);

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedDifferentValueStringLists2_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            //setup lists
            List<string> list1 = new List<string>
            {
                "123456789",
                "987654321"
            };
            List<string> list2 = new List<string>
            {
                "123456789",
                "123456789"
            };
            bool result = true;
            Action action1 = () => result = helper.AreListsEqual(list1, list2);

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedDifferentValueStringLists_ReturnsFalse()
        {
            IHelper helper = GetHelper();

            //setup lists
            List<string> list1 = new List<string>
            {
                "123456789",
                "123456789"
            };
            List<string> list2 = new List<string>
            {
                "123456789",
                "987654321"
            };
            bool result = true;
            Action action1 = () => result = helper.AreListsEqual(list1, list2);

            action1.ShouldNotThrow();
            result.Should().BeFalse();
        }
        #endregion AreListsEqual

        #region RemoveSpecialCharacters
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_RemoveSpecialCharacters_Success()
        {
            IHelper helper = GetHelper();

            string input = @"I<>|:/\?";
            string output = string.Empty;
            Action action1 = () => output = helper.RemoveSpecialCharacters(input);

            action1.ShouldNotThrow();
            output.Should().Be("I");
        }
        #endregion RemoveSpecialCharacters

        #region ExponentialDelayAsync
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExponentialDelayAsync_Success()
        {
            IHelper helper = GetHelper();

            Func<Task> action1 = async () => await helper.ExponentialDelayAsync(1, 1, 1, CancellationToken.None);
            action1.ShouldNotThrow();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExponentialDelayAsync_ExceedsMaxBackoff_Success()
        {
            IHelper helper = GetHelper();

            Func<Task> action1 = async () => await helper.ExponentialDelayAsync(500, 5, 1, CancellationToken.None);
            action1.ShouldNotThrow();
        }
        #endregion ExponentialDelayAsync

        #region ExecuteRestRequestAsync        
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_Success()
        {
            IHelper helper = new TestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, null);

            action1.ShouldNotThrow();
            result.Should().NotBeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_RetryableStatusCode_Success()
        {
            IHelper helper = new ErrorCodeTestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, null);

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_RetryableWebException_Success()
        {
            IHelper helper = new WebExceptionTestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, null);

            action1.ShouldNotThrow();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_ErrorException_Success()
        {
            IHelper helper = new ErrorExceptionTestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, null);

            action1.ShouldThrow<ArgumentNullException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_UnauthorizedNoCallback_ThrowsException()
        {
            IHelper helper = new UnauthorizedTestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, null);

            action1.ShouldThrow<UnauthorizedAccessException>();
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExecuteRestRequestAsync_Testable_UnauthorizedWithCallback_Success()
        {
            int loginFailures = 0;
            IHelper helper = new UnauthorizedTestableHelper();

            Token result = null;
            Func<Task> action1 = async () => result = await helper.ExecuteRestRequestAsync<Token>(_restClient, new RestRequest(Method.GET), _jsonSerializerSettings, 1, 1, CancellationToken.None, async () => { await Task.Delay(TimeSpan.FromMilliseconds(1)); loginFailures++; });

            action1.ShouldNotThrow();
            result.Should().BeNull();
            loginFailures.Should().BeGreaterThan(0);
        }
        #endregion ExecuteRestRequestAsync
    }
}
