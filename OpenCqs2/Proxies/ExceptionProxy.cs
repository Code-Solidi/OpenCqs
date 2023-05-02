using OpenCqs2.Policies;
using OpenCqs2.Policies.Exceptions;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class ExceptionProxy<T> : DispatchProxy
    {
        private T? target;
        private ExceptionPolicy policy = null!;

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

        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, ExceptionProxy<T>>();
            if (proxy != null)
            {
                var exceptionProxy = (ExceptionProxy<T?>)proxy;
                exceptionProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        private void Initialize(T decorated, IPolicy policy)
        {
            this.target = decorated;
            this.policy = (ExceptionPolicy)policy;
            this.policy.Initialize<T>();
        }
    }
}
