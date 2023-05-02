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
    public class QueryHandler : IQueryHandler<TestQuery, string?>
    {
        HandlerResult<string?> IQueryHandler<TestQuery, string?>.Handle(TestQuery query)
        {
            return new HandlerResult<string?> { Result = $"*** {query.Text} ***" };
        }
    }

    public class QueryWithValueHandler : IQueryHandler<TestWithValueQuery, string?>
    {
        private readonly IValueProvider valueProvider;

        public QueryWithValueHandler(IServiceProvider services)
        {
            this.valueProvider = services.GetRequiredService<IValueProvider>();
        }

        HandlerResult<string?> IQueryHandler<TestWithValueQuery, string?>.Handle(TestWithValueQuery query)
        {
            var result = query.Value + this.valueProvider.Value;
            return new HandlerResult<string?> { Result = $"*** {result} ***" };
        }
    }

    public class DecoratedQueryHandler : IQueryHandler<DecoratedTestQuery, string?>
    {
        internal static IQueryHandler<DecoratedTestQuery, string?> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return LoggingProxy<IQueryHandler<DecoratedTestQuery, string?>>.Create(new DecoratedQueryHandler(), new DemoLoggingPolicy(provider))!;
        }

        HandlerResult<string?> IQueryHandler<DecoratedTestQuery, string?>.Handle(DecoratedTestQuery query)
        {
            var result = query.Text;
            return new HandlerResult<string?> { Result = $"*** {result} ***" };
        }
    }

    public class DivisionByZeroQueryHandler : IQueryHandler<DivisionByZeroQuery, int>
    {
        internal static IQueryHandler<DivisionByZeroQuery, int> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return ExceptionProxy<IQueryHandler<DivisionByZeroQuery, int>>.Create(new DivisionByZeroQueryHandler(), new DemoExceptionPolicy(provider))!;
        }

        HandlerResult<int> IQueryHandler<DivisionByZeroQuery, int>.Handle(DivisionByZeroQuery query)
        {
            return new HandlerResult<int> { Result = 123 / query.Zero };
        }
    }
}