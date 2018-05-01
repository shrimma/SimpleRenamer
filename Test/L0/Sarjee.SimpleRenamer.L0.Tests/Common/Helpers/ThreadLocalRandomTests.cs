using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Helpers;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Helpers
{
    [TestClass]
    public class ThreadLocalRandomTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NewRandom_Success()
        {
            Random random = null;
            Action action1 = () => random = ThreadLocalRandom.NewRandom();

            action1.Should().NotThrow();
            random.Should().NotBeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_Instance_Success()
        {
            Random random = null;
            Action action1 = () => random = ThreadLocalRandom.Instance;

            action1.Should().NotThrow();
            random.Should().NotBeNull();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NextInt_Success()
        {
            int? result = null;
            Action action1 = () => result = ThreadLocalRandom.Next();

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.HasValue.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NextInt_Max_Success()
        {
            int? result = null;
            Action action1 = () => result = ThreadLocalRandom.Next(10);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.HasValue.Should().BeTrue();
            result.Value.Should().BeLessOrEqualTo(10);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NextInt_MinMax_Success()
        {
            int? result = null;
            Action action1 = () => result = ThreadLocalRandom.Next(5, 10);

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.HasValue.Should().BeTrue();
            result.Value.Should().BeLessOrEqualTo(10);
            result.Value.Should().BeGreaterOrEqualTo(5);
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NextDouble_Success()
        {
            double? result = null;
            Action action1 = () => result = ThreadLocalRandom.NextDouble();

            action1.Should().NotThrow();
            result.Should().NotBeNull();
            result.HasValue.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void ThreadLocalRandom_NextBytes_Success()
        {
            byte[] bytes = new byte[5] { 0, 0, 0, 0, 0 };
            Action action1 = () => ThreadLocalRandom.NextBytes(bytes);

            action1.Should().NotThrow();
            bytes.Should().NotBeNull();
        }
    }
}
