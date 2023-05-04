using OpenCqs2.Policies;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class DefaultExceptionProxy<T> : DispatchProxy
    {
        private T? target;
        private DefaultExceptionPolicy policy = null!;

        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, DefaultExceptionProxy<T>>();
            if (proxy != null)
            {
                var exceptionProxy = (DefaultExceptionProxy<T?>)proxy;
                exceptionProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            try
            {
                return targetMethod?.Invoke(this.target, args);
            }
            catch (Exception x)
            {
                var shouldRethrow = this.policy.Handle(x, out var wrapper);
                if (shouldRethrow)
                {
                    throw wrapper;
                }
            }

            return default;
        }

        private void Initialize(T decorated, IPolicy policy)
        {
            this.target = decorated;
            this.policy = (DefaultExceptionPolicy)policy;
            this.policy.Initialize<T>();
        }
    }
}
