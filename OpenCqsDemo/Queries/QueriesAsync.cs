/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.Logging;

using OpenCqs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCqsDemo.Queries
{
    internal class QueryHandlerAsync : QueryHandlerBaseAsync<TestQueryAsync, string>
    {
        public override async Task<string> HandleAsync(TestQueryAsync query)
        {
            return await Task.FromResult($"*** {query.Text} ***");
        }
    }

    internal class QueryWithValueHandlerAsync : QueryHandlerBaseAsync<TestWithValueQueryAsync, string>
    {
        private readonly IValueProvider valueProvider;

        public QueryWithValueHandlerAsync(IValueProvider valueProvider)
        {
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
        }

        public override async Task<string> HandleAsync(TestWithValueQueryAsync query)
        {
            var result = query.Value + this.valueProvider.Value;
            return await Task.FromResult($"*** {result.ToString()} ***");
        }
    }

    [Decorator]
    internal class LoggingQueryHandlerAsync : QueryHandlerBaseAsync<DecoratedTestQueryAsync, string>
    {
        public override async Task<string> HandleAsync(DecoratedTestQueryAsync query)
        {
            Console.WriteLine($">>>{this.Name}");
            var result = await this.next?.HandleAsync(query);
            Console.WriteLine($"<<<{this.Name}");
            return result;
        }
    }

    [Decorator]
    internal class ExceptionQueryHandlerAsync : QueryHandlerBaseAsync<DecoratedTestQueryAsync, string>
    {
        public override async Task<string> HandleAsync(DecoratedTestQueryAsync query)
        {
            Console.WriteLine($">>>{this.Name}");
            var result = await this.next?.HandleAsync(query);
            Console.WriteLine($"<<<{this.Name}");
            return result;
        }
    }

    internal class DecoratedQueryHandlerAsync : QueryHandlerBaseAsync<DecoratedTestQueryAsync, string>
    {
        public DecoratedQueryHandlerAsync()
        {
            this.Add(new ExceptionQueryHandlerAsync());   // closer
            this.Add(new LoggingQueryHandlerAsync());     // farther
        }

        public override async Task<string> HandleAsync(DecoratedTestQueryAsync query)
        {
            return await Task.FromResult($"*** {query.Text} ***");
        }
    }

    [Decorator]
    internal class DivisionByZeroLoggingQueryHandlerAsync : QueryHandlerBaseAsync<DivisionByZeroQueryAsync, int>
    {
        private readonly ILogger logger;

        public DivisionByZeroLoggingQueryHandlerAsync(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<int> HandleAsync(DivisionByZeroQueryAsync query)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = await this.next.HandleAsync(query);
            this.logger.LogInformation($"<<<{this.Name}");
            return result;
        }
    }

    [Decorator]
    internal class DivisionByZeroExceptionQueryHandlerAsync : QueryHandlerBaseAsync<DivisionByZeroQueryAsync, int>
    {
        private readonly ILogger logger;

        protected IEnumerable<Type> ExceptionTypes { get; }

        public DivisionByZeroExceptionQueryHandlerAsync(IEnumerable<Type> exceptionTypes, ILogger logger)
        {
            this.ExceptionTypes = exceptionTypes;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<int> HandleAsync(DivisionByZeroQueryAsync query)
        {
            this.logger.LogInformation($">>>{this.Name}");

            var result = default(int);
            try
            {
                result = await this.next.HandleAsync(query);
            }
            catch (Exception x)
            {
                if (!this.HandleException(x)) { throw; }
            }

            this.logger.LogInformation($"<<<{this.Name}");

            return result;
        }

        protected bool HandleException(Exception ex)
        {
            if (this.ExceptionTypes == default || this.ExceptionTypes != default && this.ExceptionTypes.Any(x => x == ex.GetType()))
            {
                this.logger.LogError(ex, string.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    internal class DivisionByZeroQueryHandlerAsync : QueryHandlerBaseAsync<DivisionByZeroQueryAsync, int>
    {
        public DivisionByZeroQueryHandlerAsync(ILogger logger)
        {
            this.Add(new DivisionByZeroExceptionQueryHandlerAsync(new[] { typeof(DivideByZeroException) }, logger));   // closer
            this.Add(new DivisionByZeroLoggingQueryHandlerAsync(logger));     // farther
        }

        public override async Task<int> HandleAsync(DivisionByZeroQueryAsync query) => await Task.FromResult(123 / query.Zero);
    }
}