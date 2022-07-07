/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenCqs;

using OpenCqsDemo.Commands;
using OpenCqsDemo.Queries;

using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenCqsDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = Program.RegisterServices(args);
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
                var testHandler = serviceProvider.GetService<IQueryHandler<TestQuery, string>>();
                var testResult = testHandler.Handle(new TestQuery { Text = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult);

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetService<IQueryHandler<TestWithValueQuery, string>>();
                var testWithValueResult = testWithValueHandler.Handle(new TestWithValueQuery { Value = 123 });
                Console.WriteLine(testWithValueResult);

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetService<IQueryHandler<DecoratedTestQuery, string>>();
                var decoratedTestResult = decoratedTestHandler.Handle(new DecoratedTestQuery { Text = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetService<IQueryHandler<DivisionByZeroQuery, int>>();
                _ = divizionByZeroHandler.Handle(new DivisionByZeroQuery());
            }
        }

        private static async Task QueryHandlingAsync(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}QueryHandling Async:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetService<IQueryHandlerAsync<TestQueryAsync, string>>();
                var testResult = await testHandler.HandleAsync(new TestQueryAsync { Text = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult);

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetService<IQueryHandlerAsync<TestWithValueQueryAsync, string>>();
                var testWithValueResult = await testWithValueHandler.HandleAsync(new TestWithValueQueryAsync { Value = 123 });
                Console.WriteLine(testWithValueResult);

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetService<IQueryHandlerAsync<DecoratedTestQueryAsync, string>>();
                var decoratedTestResult = await decoratedTestHandler.HandleAsync(new DecoratedTestQueryAsync { Text = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetService<IQueryHandlerAsync<DivisionByZeroQueryAsync, int>>();
                _ = await divizionByZeroHandler.HandleAsync(new DivisionByZeroQueryAsync());
            }
        }

        private static void CommandHandling(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}CommandHandling:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetService<ICommandHandler<TestCommand, CommandResult>>();
                var testResult = testHandler.Handle(new TestCommand { Arg = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult.ToString());

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetService<ICommandHandler<TestWithValueCommand, CommandResult>>();
                var testWithValueResult = testWithValueHandler.Handle(new TestWithValueCommand { Value = 321 });
                Console.WriteLine(testWithValueResult.ToString());

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetService<ICommandHandler<DecoratedTestCommand, CommandResult>>();
                var decoratedTestResult = decoratedTestHandler.Handle(new DecoratedTestCommand { Arg = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetService<ICommandHandler<DivisionByZeroCommand, CommandResult>>();
                _ = divizionByZeroHandler.Handle(new DivisionByZeroCommand());
            }
        }

        private static async Task CommandHandlingAsync(IServiceCollection services)
        {
            Console.WriteLine($"{Environment.NewLine}CommandHandling Async:");
            using (var serviceProvider = services.BuildServiceProvider())
            {
                Console.WriteLine(new string('-', 80));
                var testHandler = serviceProvider.GetService<ICommandHandlerAsync<TestCommandAsync, CommandResult>>();
                var testResult = await testHandler.HandleAsync(new TestCommandAsync { Arg = "Lorem ipsum dolor sit amet" });
                Console.WriteLine(testResult.ToString());

                Console.WriteLine(new string('-', 80));
                var testWithValueHandler = serviceProvider.GetService<ICommandHandlerAsync<TestWithValueCommandAsync, CommandResult>>();
                var testWithValueResult = await testWithValueHandler.HandleAsync(new TestWithValueCommandAsync { Value = 321 });
                Console.WriteLine(testWithValueResult.ToString());

                Console.WriteLine(new string('-', 80));
                var decoratedTestHandler = serviceProvider.GetService<ICommandHandlerAsync<DecoratedTestCommandAsync, CommandResult>>();
                var decoratedTestResult = await decoratedTestHandler.HandleAsync(new DecoratedTestCommandAsync { Arg = "Black Bird" });
                Console.WriteLine(decoratedTestResult);

                Console.WriteLine(new string('-', 80));
                var divizionByZeroHandler = serviceProvider.GetService<ICommandHandlerAsync<DivisionByZeroCommandAsync, CommandResult>>();
                _ = await divizionByZeroHandler.HandleAsync(new DivisionByZeroCommandAsync());
            }
        }

        private static IServiceCollection RegisterServices(string[] args)
        {
            var configuration = SetupConfiguration(args);
            var services = new ServiceCollection();

            // a service used in DemoQueryHandler (ctor DI in DemoQueryHandler)
            services.AddScoped<IValueProvider, IntValueProvider>();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                if (Enum.TryParse<LogLevel>(configuration["Logging:LogLevel:Default"], out var logLevel))
                {
                    builder.SetMinimumLevel(logLevel);
                }

                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger<ILogger>();
            services.AddSingleton<ILogger>(logger);

            return services;
        }

        private static IConfiguration SetupConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
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
