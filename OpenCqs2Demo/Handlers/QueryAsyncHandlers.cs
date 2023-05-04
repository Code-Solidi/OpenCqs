/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.DependencyInjection;

using OpenCqs2.Abstractions;
using OpenCqs2.Proxies;

using OpenCqs2Demo.Queries;

namespace OpenCqs2Demo.Handlers
{
    public class QueryHandlerAsync : IQueryHandlerAsync<TestQueryAsync, string?>
    {
        async Task<HandlerResult<string?>> IQueryHandlerAsync<TestQueryAsync, string?>.HandleAsync(TestQueryAsync query)
        {
            return await Task.FromResult(new HandlerResult<string?> { Result = $"*** {query.Text} ***" });
        }
    }

    public class QueryWithValueHandlerAsync : IQueryHandlerAsync<TestWithValueQueryAsync, string>
    {
        private readonly IValueProvider valueProvider;

        public QueryWithValueHandlerAsync(IServiceProvider services)
        {
            this.valueProvider = services.GetRequiredService<IValueProvider>();
        }

        async Task<HandlerResult<string>> IQueryHandlerAsync<TestWithValueQueryAsync, string>.HandleAsync(TestWithValueQueryAsync query)
        {
            var result = query.Value + this.valueProvider.Value;
            return await Task.FromResult(new HandlerResult<string> { Result = $"*** {result} ***" });
        }
    }

    public class DecoratedQueryHandlerAsync : IQueryHandlerAsync<DecoratedTestQueryAsync, string?>
    {
        internal static IQueryHandlerAsync<DecoratedTestQueryAsync, string?> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultLoggingProxy<IQueryHandlerAsync<DecoratedTestQueryAsync, string?>>.Create(new DecoratedQueryHandlerAsync(), new DemoLoggingPolicy(provider))!;
        }


        async Task<HandlerResult<string?>> IQueryHandlerAsync<DecoratedTestQueryAsync, string?>.HandleAsync(DecoratedTestQueryAsync query)
        {
            return await Task.FromResult(new HandlerResult<string?> { Result = $"*** {query.Text} ***" });
        }
    }

    public class DivisionByZeroQueryHandlerAsync : IQueryHandlerAsync<DivisionByZeroQueryAsync, int>
    {
        internal static IQueryHandlerAsync<DivisionByZeroQueryAsync, int> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultExceptionProxyAsync<IQueryHandlerAsync<DivisionByZeroQueryAsync, int>>.Create(new DivisionByZeroQueryHandlerAsync(), new DemoExceptionPolicy(provider/*, true*/))!;
        }

        async Task<HandlerResult<int>> IQueryHandlerAsync<DivisionByZeroQueryAsync, int>.HandleAsync(DivisionByZeroQueryAsync query)
        {
            return await Task.Run(() => new HandlerResult<int> { Result = 123 / query.Zero });
        }
    }
}