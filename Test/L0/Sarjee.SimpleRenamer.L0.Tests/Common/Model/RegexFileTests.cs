using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class RegexFileTests
    {
        private RegexFile GetRegexFile()
        {
            RegexFile regexFile = new RegexFile();
            regexFile.Should().NotBeNull();
            return regexFile;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RegexFileCtor_Success()
        {
            RegexFile regexFile = null;
            Action action1 = () => regexFile = GetRegexFile();

            action1.ShouldNotThrow();
            regexFile.Should().NotBeNull();
            regexFile.RegexExpressions.Should().NotBeNull();
        }
        #endregion Constructor

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RegexFile_RegexExpressions_Success()
        {
            RegexFile regexFile = GetRegexFile();
            Action action1 = () => regexFile.RegexExpressions = new List<RegexExpression>();

            action1.ShouldNotThrow();
            regexFile.RegexExpressions.Should().NotBeNull();
        }
    }
}
