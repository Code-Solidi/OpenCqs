using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenCqs.Tests
{
    [TestClass()]
    public class OpenCqsExtensionTests
    {
        [TestMethod()]
        public void AddHandlers_ServicesAreEmpty_WhenAssemblyHasNoCqs()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddHandlers(typeof(string).Assembly);

            // Assert
            Assert.AreEqual(services.Count, 0);
        }

        [TestMethod()]
        public void AddHandlers_ServicesAreNotEmpty_WhenAssemblyHasCqs()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(new LoggerFactory().CreateLogger<ILogger>());

            // Act
            services.AddHandlers(this.GetType().Assembly);

            // Assert
            Assert.IsTrue(services.Count > 0);
        }

        //[TestMethod()]
        //public void AddHandlers_ServicesAreNotEmpty_WhenAssemblyHasDecoratedCqs()
        //{
        //    // Arrange
        //    var services = new ServiceCollection();
        //    services.AddSingleton<ILogger>(new LoggerFactory().CreateLogger<ILogger>());

        //    // Act
        //    services.AddHandlers(this.GetType().Assembly);

        //    // Assert
        //    Assert.IsTrue(services.Count > 0);
        //}
    }

    // test queries & handlers
    [Decorator]
    internal class LoggingTestQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        private readonly ILogger logger;

        public LoggingTestQueryHandler(ILogger logger)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override string Handle(DecoratedTestQuery query)
        {
            this.logger.LogInformation("Before");
            var result = this.next.Handle(query);
            this.logger.LogInformation("Before");
            return result;
        }
    }

    [Decorator]
    internal class FakeTestQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        public override string Handle(DecoratedTestQuery query)
        {
            return this.next.Handle(query);
        }
    }

    internal class DecoratedTestQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        public DecoratedTestQueryHandler(ILogger logger)
        {
            this.Add(new LoggingTestQueryHandler(logger));
            this.Add(new FakeTestQueryHandler());
        }

        public override string Handle(DecoratedTestQuery query)
        {
            return $"*** {query.Text} ***";
        }
    }

    internal class DecoratedTestQuery : IQuery
    {
        public string Text { get; set; } = string.Empty;
    }

    internal class TestQueryHandler : QueryHandlerBase<TestQuery, string>
    {
        public override string Handle(TestQuery query)
        {
            return $"*** {query.Text} ***";
        }
    }

    internal class TestQuery : IQuery
    {
        public string Text { get; set; } = string.Empty;
    }
}