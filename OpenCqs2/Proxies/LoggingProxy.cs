using Microsoft.Extensions.Logging;

using OpenCqs2.Policies;
using OpenCqs2.Policies.Logging;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class LoggingProxy<T> : DispatchProxy
    {
        private T? target;
        private LoggingPolicy? policy;

        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, LoggingProxy<T>>();
            if (proxy != null)
            {
                var loggingProxy = (LoggingProxy<T?>)proxy;
                loggingProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            this.policy?.Logger.LogInformation($"Calling method {targetMethod?.Name} with arguments {string.Join(",", args ?? Array.Empty<object>())}");
            var result = targetMethod?.Invoke(this.target, args);
            this.policy?.Logger?.LogInformation($"Called method {targetMethod?.Name} with result {result}");
            return result;
        }

        private void Initialize(T decorated, IPolicy policy)
        {
            this.target = decorated;
            this.policy = (LoggingPolicy)policy;
            this.policy.Initialize<T>();
        }
    }
}
