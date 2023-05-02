using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using OpenCqs2.Policies.Exceptions;

using T = System.String;

namespace OpenCqs2.Tests.Policies.Exceptions
{
    [TestClass]
    public class ExceptionPolicyTests
    {
        private ExceptionPolicy testClass = null!;
        private IServiceProvider provider = null!;

        [TestInitialize]
        public void SetUp()
        {
            var services = new ServiceCollection();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            services.AddSingleton(loggerFactory);
            this.provider = services.BuildServiceProvider();
            this.testClass = new ExceptionPolicy(this.provider);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new ExceptionPolicy(this.provider);

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CannotConstructWithNullProvider()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ExceptionPolicy(default!));
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
        public void CanCallHandle()
        {
            // Arrange
            var x = new Exception();

            // Act
            var result = this.testClass.Handle(x, out var wrapper);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(x, wrapper);
        }

        [TestMethod]
        public void CannotCallHandleWithNullX()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testClass.Handle(default!, out _));
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