using Microsoft.Extensions.Logging;

using OpenCqs2.Policies.Exceptions;
using OpenCqs2.Policies.Logging;

namespace OpenCqs2Demo.Handlers
{
    internal class DemoLoggingPolicy : LoggingPolicy
    {
        public DemoLoggingPolicy(IServiceProvider provider) : base(provider)
        {
        }

        public override void LogMessage(string message)
        {
            this.Logger.LogDebug(message);
        }
    }

    internal class DemoExceptionPolicy : ExceptionPolicy
    {
        private readonly bool reThrow;

        public DemoExceptionPolicy(IServiceProvider provider, bool reThrow = false) : base(provider)
        {
            this.reThrow = reThrow;
        }

        public override bool Handle(Exception x, out Exception wrapper)
        {
            wrapper = x.InnerException ?? x; // or something else with a customised error message which may or may not inlclude the original exception
            this.Logger.LogError($"{wrapper.GetType().FullName}: {wrapper.Message}");
            return this.reThrow;
        }
    }
}
