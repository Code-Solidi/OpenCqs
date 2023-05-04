using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using OpenCqs2.Policies;

using System;

using T = System.String;

namespace OpenCqs2.Tests.Policies
{
    [TestClass]
    public class DefaultLoggingPolicyTests
    {
        private DefaultLoggingPolicy testClass = null!;
        private IServiceProvider provider = null!;

        [TestInitialize]
        public void SetUp()
        {
            var services = new ServiceCollection();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            services.AddSingleton(loggerFactory);
            this.provider = services.BuildServiceProvider();
            this.testClass = new DefaultLoggingPolicy(this.provider);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new DefaultLoggingPolicy(this.provider);

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CannotConstructWithNullProvider()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultLoggingPolicy(default(IServiceProvider)));
        }

        [TestMethod]
        public void CanCallInitialize()
        {
            // Act
            this.testClass.Initialize<T>();

            // Assert
            Assert.IsNotNull(this.testClass.Logger);
        }

        [TestMethod]
        public void CanCallLogMessage()
        {
            // Arrange
            this.testClass.Initialize<T>();
            var message = "TestValue582430697";

            // Act
            this.testClass.LogMessage(message);

            // Assert
            //Assert.Fail("Create or modify test");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void CannotCallLogMessageWithInvalidMessage(string value)
        {
            // Arrange
            this.testClass.Initialize<T>();

            // Assert
            Assert.ThrowsException<ArgumentException>(() => this.testClass.LogMessage(value));
        }

        [TestMethod]
        public void CanGetLogger()
        {
            // Arrange
            this.testClass.Initialize<T>();

            // Assert
            Assert.IsInstanceOfType(this.testClass.Logger, typeof(ILogger));
        }
    }
}