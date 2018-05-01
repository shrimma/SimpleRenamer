using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class RegexExpressionTests
    {
        private RegexExpression GetRegexExpression(string expression = "expression", bool isEnabled = true, bool isForTv = true)
        {
            RegexExpression regexExpression = new RegexExpression(expression, isEnabled, isForTv);
            regexExpression.Should().NotBeNull();
            return regexExpression;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RegexExpressionCtor_NullArguments_ThrowArgumentNullExceptions()
        {
            Action action1 = () => new RegexExpression(string.Empty, false, false);

            action1.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void RegexExpressionCtor_Success()
        {
            RegexExpression regexExpression = null;
            Action action1 = () => regexExpression = GetRegexExpression();

            action1.Should().NotThrow();
            regexExpression.Should().NotBeNull();
            regexExpression.Expression.Should().Be("expression");
            regexExpression.IsEnabled.Should().BeTrue();
            regexExpression.IsForTvShow.Should().BeTrue();
        }
        #endregion Constructor
    }
}
