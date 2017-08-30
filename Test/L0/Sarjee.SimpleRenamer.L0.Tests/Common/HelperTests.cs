using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class HelperTests
    {
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

            Func<Task> action1 = async () => await helper.ExponentialDelayAsync(1, 1, 1);
            action1.ShouldNotThrow();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_ExponentialDelayAsync_ExceedsMaxBackoff_Success()
        {
            IHelper helper = GetHelper();

            Func<Task> action1 = async () => await helper.ExponentialDelayAsync(500, 5, 1);
            action1.ShouldNotThrow();
        }
        #endregion ExponentialDelayAsync
    }
}
