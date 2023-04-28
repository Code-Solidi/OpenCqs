using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenCqs2.Policies.Exceptions
{
    public class ExceptionPolicy : IPolicy
    {
        public ExceptionPolicy(IServiceProvider provider)
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

        internal virtual bool Handle(Exception exception, out Exception translated)
        {
            this.Logger.LogError(exception.Message, exception);
            translated = exception;
            return true;
        }
    }
}