using OpenCqs2.Policies;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class DefaultLoggingProxy<T> : DispatchProxy
    {
        private T? target;
        private DefaultLoggingPolicy? policy;

        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, DefaultLoggingProxy<T>>();
            if (proxy != null)
            {
                var loggingProxy = (DefaultLoggingProxy<T?>)proxy;
                loggingProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            this.policy?.LogMessage($"Calling method {targetMethod?.Name} with arguments {string.Join(",", args ?? Array.Empty<object>())}");
            //this.policy?.Logger.LogInformation($"Calling method {targetMethod?.Name} with arguments {string.Join(",", args ?? Array.Empty<object>())}");
            var result = targetMethod?.Invoke(this.target, args);
            //this.policy?.Logger?.LogInformation($"Called method {targetMethod?.Name} with result {result}");
            this.policy?.LogMessage($"Called method {targetMethod?.Name} with result {result}");
            return result;
        }

        private void Initialize(T decorated, IPolicy policy)
        {
            this.target = decorated;
            this.policy = (DefaultLoggingPolicy)policy;
            this.policy.Initialize<T>();
        }
    }
}
