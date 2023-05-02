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

        public virtual bool Handle(Exception x, out Exception wrapper)
        {
            wrapper = x ?? throw new ArgumentNullException(nameof(x));
            return true;
        }
    }
}