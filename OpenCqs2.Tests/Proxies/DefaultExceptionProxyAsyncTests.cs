using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using OpenCqs2.Abstractions;
using OpenCqs2.Policies;
using OpenCqs2.Proxies;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using R = System.String;

using T = System.String;

namespace OpenCqs2.Tests.Proxies
{
    [TestClass]
    public class DefaultExceptionProxyAsync_1Tests
    {
        private IServiceCollection services = null!;

        [TestInitialize]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            this.services = new ServiceCollection();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                if (Enum.TryParse<LogLevel>(configuration["Logging:LogLevel:Default"], out var logLevel))
                {
                    builder.SetMinimumLevel(logLevel);
                }

                builder.AddConsole();
            });

            this.services.AddSingleton(loggerFactory);
            this.services.AddHandlers(this.GetType().Assembly);
        }

        [TestMethod]
        public void CanCallCreate()
        {
            // Arrange
            var provider = this.services.BuildServiceProvider();

            // Act
            var result = provider.GetRequiredService<IQueryHandler<QueryWithExceptionProxy, string?>>();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CanCallInvokeAsync()
        {
            // Arrange
            var provider = this.services.BuildServiceProvider();
            var handler = provider.GetRequiredService<IQueryHandler<QueryWithExceptionProxy, string?>>();

            // Act
            var result = handler.Handle(new QueryWithExceptionProxy());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(HandlerResult<string?>));
        }

        /*
        [TestMethod]
        public void CanCallInvoke()
        {
            // Arrange
            var method = this.testClass.GetType().GetMethod("CreateProxy");
            var args = new[] { new object(), new object(), new object() };

            // Act
            var result = this.testClass.Invoke(method!, args);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CannotCallInvokeWithNullMethod()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.testClass.Invoke(default(MethodInfo), new[] { new object(), new object(), new object() }));
        }

        [TestMethod]
        public void CannotCallInvokeWithNullArgs()
        {
            // Arrange
            var method = this.testClass.GetType().GetMethod("CreateProxy");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => this.testClass.Invoke(method, default(object[])));
        }

        [TestMethod]
        public async Task CanCallInvokeAsync()
        {
            // Arrange
            var targetMethod = this.testClass.GetType().GetMethod("CreateProxy");
            var args = new[] { new object(), new object(), new object() };

            // Act
            await this.testClass.InvokeAsync(targetMethod, args);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestMethod]
        public async Task CannotCallInvokeAsyncWithNullTargetMethod()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => this.testClass.InvokeAsync(default(MethodInfo), new[] { new object(), new object(), new object() }));
        }

        [TestMethod]
        public async Task CannotCallInvokeAsyncWithNullArgs()
        {
            // Arrange
            var method = this.testClass.GetType().GetMethod("CreateProxy");

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => this.testClass.InvokeAsync(method, default(object[])));
        }

        [TestMethod]
        public async Task CanCallInvokeAsyncT()
        {
            // Arrange
            var targetMethod = this.testClass.GetType().GetMethod("CreateProxy");
            var args = new[] { new object(), new object(), new object() };

            // Act
            var result = await this.testClass.InvokeAsyncT<R>(targetMethod, args);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestMethod]
        public async Task CannotCallInvokeAsyncTWithNullTargetMethod()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => this.testClass.InvokeAsyncT<R>(default(MethodInfo), new[] { new object(), new object(), new object() }));
        }

        [TestMethod]
        public async Task CannotCallInvokeAsyncTWithNullArgs()
        {
            // Arrange
            var method = this.testClass.GetType().GetMethod("CreateProxy");

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => this.testClass.InvokeAsyncT<R>(method, default(object[])));
        }*/
    }
}