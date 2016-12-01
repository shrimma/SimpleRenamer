using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common;
using Sarjee.SimpleRenamer.Common.Interface;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.L0.Tests.Common
{
    [TestClass]
    public class HelperTests
    {
        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void HelperCtor_Success()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);
        }
        #endregion Constructor

        #region IsFileExtensionValid
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_Empty_ReturnsFalse()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            bool result = helper.IsFileExtensionValid(string.Empty);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_JustFullStop_ReturnsFalse()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            bool result = helper.IsFileExtensionValid(".");

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_MissingFullStop_ReturnsFalse()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            bool result = helper.IsFileExtensionValid("fil");

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_IsFileExtensionValid_Valid_ReturnsTrue()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            bool result = helper.IsFileExtensionValid(".fil");

            Assert.IsTrue(result);
        }
        #endregion IsFileExtensionValid

        #region AreListsEqual
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_EmptyStringLists_ReturnsTrue()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            //setup lists
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();

            bool result = helper.AreListsEqual(list1, list2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedEqualStringLists_ReturnsTrue()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            //setup lists
            List<string> list1 = new List<string>();
            list1.Add("123456789");
            List<string> list2 = new List<string>();
            list2.Add("123456789");
            list2.Add("1");
            list2.Remove("1");

            bool result = helper.AreListsEqual(list1, list2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedDifferentCountStringLists_ReturnsFalse()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            //setup lists
            List<string> list1 = new List<string>();
            list1.Add("123456789");
            list1.Add("123456789");
            List<string> list2 = new List<string>();
            list2.Add("123456789");

            bool result = helper.AreListsEqual(list1, list2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void Helper_AreListsEqual_PopulatedDifferentValueStringLists_ReturnsFalse()
        {
            IHelper helper = new Helper();

            Assert.IsNotNull(helper);

            //setup lists
            List<string> list1 = new List<string>();
            list1.Add("123456789");
            list1.Add("123456789");
            List<string> list2 = new List<string>();
            list2.Add("123456789");
            list2.Add("987654321");

            bool result = helper.AreListsEqual(list1, list2);

            Assert.IsFalse(result);
        }
        #endregion AreListsEqual
    }
}
