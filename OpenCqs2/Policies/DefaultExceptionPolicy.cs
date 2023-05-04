using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenCqs2.Policies
{
    public class DefaultExceptionPolicy : IPolicy
    {
        public DefaultExceptionPolicy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        }

        private readonly ILoggerFactory loggerFactory;

        public ILogger Logger { get; private set; } = null!;

        public virtual void Initialize<T>()
        {
            Logger = loggerFactory.CreateLogger<T>();
        }

        public virtual bool Handle(Exception x, out Exception wrapper)
        {
            wrapper = x ?? throw new ArgumentNullException(nameof(x));
            return true;
        }
    }
}