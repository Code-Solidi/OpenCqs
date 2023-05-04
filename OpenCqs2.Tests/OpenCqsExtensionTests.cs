using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using OpenCqs2.Abstractions;
using OpenCqs2.Policies;
using OpenCqs2.Proxies;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OpenCqs2.Tests
{
    //public class QueryWithoutProxy : IQuery
    //{
    //}

    //public class QueryWithLoggingProxy : IQuery
    //{
    //}

    //public class QueryWithoutProxyHandler : IQueryHandler<QueryWithoutProxy, string?>
    //{
    //    HandlerResult<string?> IQueryHandler<QueryWithoutProxy, string?>.Handle(QueryWithoutProxy query)
    //    {
    //        return new HandlerResult<string?> { Result = "Result" };
    //    }
    //}

    //public class QueryWithExceptionProxyHandler : IQueryHandler<QueryWithLoggingProxy, string?>
    //{
    //    HandlerResult<string?> IQueryHandler<QueryWithLoggingProxy, string?>.Handle(QueryWithLoggingProxy query)
    //    {
    //        return new HandlerResult<string?> { Result = "Result" };
    //    }

    //    internal static IQueryHandler<QueryWithLoggingProxy, string?> CreateProxy(IServiceProvider provider)
    //    {
    //        _ = provider ?? throw new ArgumentNullException(nameof(provider));
    //        return DefaultLoggingProxy<IQueryHandler<QueryWithLoggingProxy, string?>>.Create(new QueryWithExceptionProxyHandler(), new DefaultLoggingPolicy(provider))!;
    //    }
    //}

    [TestClass]
    public class OpenCqsExtensionTests
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
        }

        [TestMethod]
        public void CanCallAddHandlersWithServicesAndAssemblies()
        {
            // Arrange
            var assemblies = new[] { this.GetType().Assembly };

            // Act
            var result = this.services.AddHandlers(assemblies);

            // Assert
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void CannotCallAddHandlersWithServicesAndAssembliesWithNullServices()
        {
            // Arrange
            var assemblies = new[] { this.GetType().Assembly };

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => default(IServiceCollection).AddHandlers(assemblies));
        }

        [TestMethod]
        public void CannotCallAddHandlersWithServicesAndAssembliesWithNullAssemblies()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Substitute.For<IServiceCollection>().AddHandlers(default(IEnumerable<Assembly>)));
        }

        [TestMethod]
        public void CanCallAddHandlersWithServicesAndAssembly()
        {
            // Arrange
            var assembly = this.GetType().Assembly;

            // Act
            var result = this.services.AddHandlers(assembly);

            // Assert
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void CanInstantiateHandlerWithoutProxy()
        {
            // Arrange
            var assembly = this.GetType().Assembly;
            var augmented = this.services.AddHandlers(assembly);

            // Act
            var result = augmented.BuildServiceProvider().GetService<IQueryHandler<QueryWithoutProxy, string?>>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(QueryWithoutProxyHandler));
        }

        [TestMethod]
        public void CanInstantiateHandlerWithProxy()
        {
            // Arrange
            var assembly = this.GetType().Assembly;
            var augmented = this.services.AddHandlers(assembly);

            // Act
            var result = augmented.BuildServiceProvider().GetService<IQueryHandler<QueryWithLoggingProxy, string?>>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType().BaseType == typeof(DefaultLoggingProxy<IQueryHandler<QueryWithLoggingProxy, string?>>));
        }

        [TestMethod]
        public void CannotCallAddHandlersWithServicesAndAssemblyWithNullServices()
        {
            Assert.ThrowsException<ArgumentNullException>(() => default(IServiceCollection).AddHandlers(Assembly.GetAssembly(typeof(string))));
        }

        [TestMethod]
        public void CannotCallAddHandlersWithServicesAndAssemblyWithNullAssembly()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Substitute.For<IServiceCollection>().AddHandlers(default(Assembly)));
        }
    }
}