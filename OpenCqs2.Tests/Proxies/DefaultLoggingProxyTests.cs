using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenCqs2.Abstractions;

using System;
using System.IO;

namespace OpenCqs2.Tests.Proxies
{
    [TestClass]
    public class DefaultLoggingProxy_1Tests
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
            var result = provider.GetRequiredService<IQueryHandler<QueryWithLoggingProxy, string?>>();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CanCallInvoke()
        {
            // Arrange
            var provider = this.services.BuildServiceProvider();
            var handler = provider.GetRequiredService<IQueryHandler<QueryWithLoggingProxy, string?>>();

            // Act
            var result = handler.Handle(new QueryWithLoggingProxy());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(HandlerResult<string?>));
        }
    }
}