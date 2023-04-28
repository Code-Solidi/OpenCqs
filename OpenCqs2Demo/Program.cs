using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenCqs2;
using OpenCqs2.Abstractions;

using OpenCqs2Demo.Demos;

namespace OpenCqs2Demo
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var services = RegisterServices(args);

            // a service used in DemoQueryHandler (ctor DI in DemoQueryHandler)
            //services.AddScoped<IValueProvider, IntValueProvider>();

            services.AddHandlers(typeof(Program).Assembly);

            var provider = services.BuildServiceProvider();
            var handler = provider.GetRequiredService<IQueryHandler<DemoQuery, string?>>();
            var query = new DemoQuery { Text = "Hello" };
            var result = handler.Handle(query);

            Console.WriteLine(result);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
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