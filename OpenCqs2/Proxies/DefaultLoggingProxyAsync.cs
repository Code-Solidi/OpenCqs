using OpenCqs2.Policies;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class DefaultLoggingProxyAsync<T> : DispatchProxyAsync
    {
        private T? target;
        private DefaultLoggingPolicy policy = null!;

        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, DefaultLoggingProxyAsync<T>>();
            if (proxy != null)
            {
                var loggingProxy = (DefaultLoggingProxyAsync<T?>)proxy;
                loggingProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            throw new NotImplementedException();
        }

        public override async Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            this.policy?.LogMessage($"Calling method {targetMethod?.Name} with arguments {string.Join(",", args ?? Array.Empty<object>())}");
            await (Task)targetMethod?.Invoke(this.target, args)!;
            this.policy?.LogMessage($"Called method {targetMethod?.Name}");
        }

        public override async Task<R> InvokeAsyncT<R>(MethodInfo targetMethod, object[] args)
        {
            this.policy?.LogMessage($"Calling method {targetMethod?.Name} with arguments {string.Join(",", args ?? Array.Empty<object>())}");
            var result = await (Task<R>)targetMethod?.Invoke(this.target, args)!;
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