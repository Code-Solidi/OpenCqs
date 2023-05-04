using OpenCqs2.Abstractions;
using OpenCqs2.Policies;
using OpenCqs2.Proxies;

using System;
using System.Threading.Tasks;

namespace OpenCqs2.Tests
{
    public class QueryWithoutProxy : IQuery
    {
    }

    public class QueryWithoutProxyAsync : IQuery
    {
    }

    public class QueryWithLoggingProxy : IQuery
    {
    }

    public class QueryWithExceptionProxy : IQuery
    {
    }

    public class QueryWithoutProxyHandler : IQueryHandler<QueryWithoutProxy, string?>
    {
        HandlerResult<string?> IQueryHandler<QueryWithoutProxy, string?>.Handle(QueryWithoutProxy query)
        {
            return new HandlerResult<string?> { Result = "Result" };
        }
    }

    public class QueryWithLoggingProxyHandler : IQueryHandler<QueryWithLoggingProxy, string?>
    {
        HandlerResult<string?> IQueryHandler<QueryWithLoggingProxy, string?>.Handle(QueryWithLoggingProxy query)
        {
            return new HandlerResult<string?> { Result = "Result" };
        }

        internal static IQueryHandler<QueryWithLoggingProxy, string?> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultLoggingProxy<IQueryHandler<QueryWithLoggingProxy, string?>>.Create(new QueryWithLoggingProxyHandler(), new DefaultLoggingPolicy(provider))!;
        }
    }

    public class QueryWithExceptionProxyHandler : IQueryHandler<QueryWithExceptionProxy, string?>
    {
        HandlerResult<string?> IQueryHandler<QueryWithExceptionProxy, string?>.Handle(QueryWithExceptionProxy query)
        {
            return new HandlerResult<string?> { Result = "Result" };
        }

        internal static IQueryHandler<QueryWithExceptionProxy, string?> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultExceptionProxy<IQueryHandler<QueryWithExceptionProxy, string?>>.Create(new QueryWithExceptionProxyHandler(), new DefaultExceptionPolicy(provider))!;
        }
    }

    public class QueryWithoutProxyHandlerAsync : IQueryHandlerAsync<QueryWithoutProxyAsync, string?>
    {
        async Task<HandlerResult<string?>> IQueryHandlerAsync<QueryWithoutProxyAsync, string?>.HandleAsync(QueryWithoutProxyAsync query)
        {
            return await Task.FromResult(new HandlerResult<string?> { Result = "Result" });
        }
    }
}