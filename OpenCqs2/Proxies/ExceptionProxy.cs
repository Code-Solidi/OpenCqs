using OpenCqs2.Policies;
using OpenCqs2.Policies.Exceptions;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class ExceptionProxy<T> : DispatchProxy
    {
        private T? decorated;
        private ExceptionPolicy? policy;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            object? result = default;
            try
            {
                result = targetMethod?.Invoke(this.decorated, args);
            }
            catch (Exception x)
            {
                var shouldRethrow = this.policy?.Handle(x, out var translated) ?? true;
                if (shouldRethrow)
                {
                    throw;
                }
            }

            return result;
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
            this.decorated = decorated;
            this.policy = (ExceptionPolicy)policy;
            this.policy.Initialize<T>();
        }
    }
}
