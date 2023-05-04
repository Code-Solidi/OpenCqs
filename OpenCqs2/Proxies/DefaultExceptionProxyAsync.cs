using OpenCqs2.Policies;

using System.Reflection;

namespace OpenCqs2.Proxies
{
    public class DefaultExceptionProxyAsync<T> : DispatchProxyAsync
    {
        private T? target;
        private DefaultExceptionPolicy policy = null!;


        public static T? Create(T target, IPolicy policy)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));
            _ = policy ?? throw new ArgumentNullException(nameof(policy));

            object? proxy = Create<T, DefaultExceptionProxyAsync<T>>();
            if (proxy != null)
            {
                var exceptionProxy = (DefaultExceptionProxyAsync<T?>)proxy;
                exceptionProxy.Initialize(target, policy);
            }

            return (T?)proxy;
        }

        public override object Invoke(MethodInfo method, object[] args)
        {
            throw new NotImplementedException();
        }

        public override async Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            try
            {
                await (Task)targetMethod.Invoke(this.target, args)!;
            }
            catch (Exception x)
            {
                var shouldRethrow = this.policy.Handle(x, out var wrapper);
                if (shouldRethrow)
                {
                    throw wrapper;
                }
            }
        }

        public override async Task<R> InvokeAsyncT<R>(MethodInfo targetMethod, object[] args)
        {
            try
            {
                return await (Task<R>)targetMethod.Invoke(this.target, args)!;
            }
            catch (Exception x)
            {
                var shouldRethrow = this.policy.Handle(x, out var wrapper);
                if (shouldRethrow)
                {
                    throw wrapper;
                }
            }

            return default!;
        }

        private void Initialize(T decorated, IPolicy policy)
        {
            this.target = decorated;
            this.policy = (DefaultExceptionPolicy)policy;
            this.policy.Initialize<T>();
        }

#if false
        /*protected override async Task<object?> Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            //var result = default(Task<HandlerResult<object>>);
            var result = default(Task<object?>);
            try
            {
                //var task = targetMethod?.Invoke(this.target, args);
                //await task.ConfigureAwait(false);
                //result = await ((Task<T>)task).Result;
                //var func = async () => await (Task<object?>)targetMethod.Invoke(this.target, args);
                //var func = targetMethod?.Invoke(this.target, args);
                var task = Task.Run(async () => await targetMethod?.Invoke(this.target, args));
                result = task.ContinueWith(t  => (object?)t.Result);
                //Task task = Task.Run(async () => await (Task)targetMethod.Invoke(this.target, args));
            }
            catch (Exception x)
            {
                var shouldRethrow = this.policy.Handle(x, out var wrapper);
                if (shouldRethrow)
                {
                    throw wrapper;
                }
            }

            return await result!;
        }*/

        //public override Task InvokeAsync(MethodInfo method, object[] args)
        //{
        //    var result = default(object);
        //    try
        //    {
        //        //Task.Run(async () => await (Task)targetMethod.Invoke(this.target, args)).GetAwaiter().GetResult();

        //        //var result2 = Task.Run(async () => await (Task)targetMethod.Invoke(this.target, args));//.GetAwaiter().GetResult();
        //        //var result3 = result2.GetType().GetProperty("Result").GetValue(result2);
        //        //return result3;
        //        result = await this.InvokeAsync(targetMethod, args);
        //        //var x = result.GetType().GetProperty("Result").GetValue(result);
        //        //return x;
        //    }
        //    catch (Exception x)
        //    {
        //        var shouldRethrow = this.policy.Handle(x, out var wrapper);
        //        if (shouldRethrow)
        //        {
        //            throw wrapper;
        //        }
        //    }

        //    return result;
        //}
#endif
    }
}
