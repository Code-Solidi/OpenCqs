using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenCqs2.Policies.Logging
{
    public class LoggingPolicy : IPolicy
    {
        public LoggingPolicy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            this.loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        }

        private readonly ILoggerFactory loggerFactory;

        public ILogger Logger { get; private set; } = null!;

        public virtual void Initialize<T>()
        {
            this.Logger = this.loggerFactory.CreateLogger<T>();
        }

        public virtual void LogMessage(string message)
        {
            this.Logger.LogInformation(message);
        }
    }
}