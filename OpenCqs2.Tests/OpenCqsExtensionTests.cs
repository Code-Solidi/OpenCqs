using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using OpenCqs2;

namespace OpenCqs2.Tests
{
    [TestClass]
    public class OpenCqsExtensionTests
    {
        [TestMethod]
        public void CanCallAddHandlersWithServicesAndAssemblies()
        {
            // Arrange
            var services = Substitute.For<IServiceCollection>();
            var assemblies = new[] { Assembly.GetAssembly(typeof(string)), Assembly.GetAssembly(typeof(string)), Assembly.GetAssembly(typeof(string)) };

            // Act
            var result = services.AddHandlers(assemblies);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestMethod]
        public void CannotCallAddHandlersWithServicesAndAssembliesWithNullServices()
        {
            Assert.ThrowsException<ArgumentNullException>(() => default(IServiceCollection).AddHandlers(new[] { Assembly.GetAssembly(typeof(string)), Assembly.GetAssembly(typeof(string)), Assembly.GetAssembly(typeof(string)) }));
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
            var services = Substitute.For<IServiceCollection>();
            var @assembly = Assembly.GetAssembly(typeof(string));

            // Act
            var result = services.AddHandlers(assembly);

            // Assert
            Assert.Fail("Create or modify test");
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

        [TestMethod]
        public void CanCallCreateProxy()
        {
            // Arrange
            var @type = typeof(string);
            var provider = Substitute.For<IServiceProvider>();

            // Act
            var result = type.CreateProxy(provider);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestMethod]
        public void CannotCallCreateProxyWithNullType()
        {
            Assert.ThrowsException<ArgumentNullException>(() => default(Type).CreateProxy(Substitute.For<IServiceProvider>()));
        }

        [TestMethod]
        public void CannotCallCreateProxyWithNullProvider()
        {
            Assert.ThrowsException<ArgumentNullException>(() => typeof(string).CreateProxy(default(IServiceProvider)));
        }
    }
}