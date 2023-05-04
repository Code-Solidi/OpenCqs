using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenCqs2;
using OpenCqs2.Abstractions;

using OpenCqs2Demo.Commands;
using OpenCqs2Demo.Queries;

namespace OpenCqs2Demo
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var services = Program.RegisterServices(args);

            // a service used in DemoQueryHandler (ctor DI in DemoQueryHandler)
            services.AddScoped<IValueProvider, IntValueProvider>();

            services.AddHandlers(typeof(Program).Assembly);

            Program.QueryHandling(services);
            Program.QueryHandlingAsync(services).GetAwaiter().GetResult();

            Program.CommandHandling(services);
            Program.CommandHandlingAsync(services).GetAwaiter().GetResult();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void QueryHandling(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}QueryHandling:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetRequiredService<IQueryHandler<TestQuery, string>>();
                var testResult = testHandler.Handle(new TestQuery { Text = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult);

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetRequiredService<IQueryHandler<TestWithValueQuery, string>>();
                var testWithValueResult = testWithValueHandler.Handle(new TestWithValueQuery { Value = 123 });
                Console.WriteLine(testWithValueResult);

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetRequiredService<IQueryHandler<DecoratedTestQuery, string>>();
                var decoratedTestResult = decoratedTestHandler.Handle(new DecoratedTestQuery { Text = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetRequiredService<IQueryHandler<DivisionByZeroQuery, int>>();
                _ = divizionByZeroHandler.Handle(new DivisionByZeroQuery());
            }
        }

        private static async Task QueryHandlingAsync(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}QueryHandling Async:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetRequiredService<IQueryHandlerAsync<TestQueryAsync, string>>();
                var testResult = await testHandler.HandleAsync(new TestQueryAsync { Text = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult);

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetRequiredService<IQueryHandlerAsync<TestWithValueQueryAsync, string>>();
                var testWithValueResult = await testWithValueHandler.HandleAsync(new TestWithValueQueryAsync { Value = 123 });
                Console.WriteLine(testWithValueResult);

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetRequiredService<IQueryHandlerAsync<DecoratedTestQueryAsync, string>>();
                var decoratedTestResult = await decoratedTestHandler.HandleAsync(new DecoratedTestQueryAsync { Text = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetRequiredService<IQueryHandlerAsync<DivisionByZeroQueryAsync, int>>();
                _ = await divizionByZeroHandler.HandleAsync(new DivisionByZeroQueryAsync());

                /*try
                {
                    _ = await divizionByZeroHandler.HandleAsync(new DivisionByZeroQueryAsync());
                }
                catch (Exception x)
                {
                    await Console.Out.WriteLineAsync($"*** {x.Message} ***");
                }*/
            }
        }

        private static void CommandHandling(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}CommandHandling:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetRequiredService<ICommandHandler<TestCommand>>();
                var testResult = testHandler?.Handle(new TestCommand { Arg = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult?.ToString());

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetRequiredService<ICommandHandler<TestWithValueCommand>>();
                var testWithValueResult = testWithValueHandler?.Handle(new TestWithValueCommand { Value = 321 });
                Console.WriteLine(testWithValueResult?.ToString());

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetRequiredService<ICommandHandler<DecoratedTestCommand>>();
                var decoratedTestResult = decoratedTestHandler?.Handle(new DecoratedTestCommand { Arg = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetRequiredService<ICommandHandler<DivisionByZeroCommand>>();
                _ = divizionByZeroHandler?.Handle(new DivisionByZeroCommand());
            }
        }

        private static async Task CommandHandlingAsync(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}CommandHandling Async:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetRequiredService<ICommandHandlerAsync<TestCommandAsync>>();
                var testResult = await testHandler.HandleAsync(new TestCommandAsync { Arg = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult.ToString());

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetRequiredService<ICommandHandlerAsync<TestWithValueCommandAsync>>();
                var testWithValueResult = await testWithValueHandler.HandleAsync(new TestWithValueCommandAsync { Value = 321 });
                Console.WriteLine(testWithValueResult.ToString());

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetRequiredService<ICommandHandlerAsync<DecoratedTestCommandAsync>>();
                var decoratedTestResult = await decoratedTestHandler.HandleAsync(new DecoratedTestCommandAsync { Arg = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetRequiredService<ICommandHandlerAsync<DivisionByZeroCommandAsync>>();
                _ = await divizionByZeroHandler.HandleAsync(new DivisionByZeroCommandAsync());
            }
        }

        private static IServiceCollection RegisterServices(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var services = new ServiceCollection();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                if (Enum.TryParse<LogLevel>(configuration["Logging:LogLevel:Default"], out var logLevel))
                {
                    builder.SetMinimumLevel(logLevel);
                }

                builder.AddConsole();
            });

            services.AddSingleton(loggerFactory);

            return services;
        }

        private class IntValueProvider : IValueProvider
        {
            public int Value => 123;
        }
    }

    public interface IValueProvider
    {
        int Value { get; }
    }
}