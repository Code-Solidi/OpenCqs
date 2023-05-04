using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenCqs2.Policies
{
    public class DefaultLoggingPolicy : IPolicy
    {
        public DefaultLoggingPolicy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        }

        private readonly ILoggerFactory loggerFactory;

        public ILogger Logger { get; private set; } = null!;

        public virtual void Initialize<T>()
        {
            this.Logger = this.loggerFactory.CreateLogger<T>();
        }

        public virtual void LogMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
            }

            this.Logger.LogInformation(message);
        }
    }
}