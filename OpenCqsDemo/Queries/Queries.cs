/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.Logging;

using OpenCqs;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCqsDemo.Queries
{
    internal class QueryHandler : QueryHandlerBase<TestQuery, string>
    {
        public override string Handle(TestQuery query)
        {
            return $"*** {query.Text} ***";
        }
    }

    internal class QueryWithValueHandler : QueryHandlerBase<TestWithValueQuery, string>
    {
        private readonly IValueProvider valueProvider;

        public QueryWithValueHandler(IValueProvider valueProvider)
        {
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
        }

        public override string Handle(TestWithValueQuery query)
        {
            var result = query.Value + this.valueProvider.Value;
            return $"*** {result.ToString()} ***";
        }
    }

    [Decorator]
    internal class LoggingQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        private readonly ILogger logger;

        public LoggingQueryHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override string Handle(DecoratedTestQuery query)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = this.next?.Handle(query);
            this.logger.LogInformation($"<<<{this.Name}");
            return result;
        }
    }

    [Decorator]
    internal class ExceptionQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        private readonly ILogger logger;

        public ExceptionQueryHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override string Handle(DecoratedTestQuery query)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = this.next?.Handle(query);
            this.logger.LogInformation($"<<<{this.Name}");
            return result;
        }
    }

    internal class DecoratedQueryHandler : QueryHandlerBase<DecoratedTestQuery, string>
    {
        public DecoratedQueryHandler(ILogger logger)
        {
            this.Add(new ExceptionQueryHandler(logger));   // closer
            this.Add(new LoggingQueryHandler(logger));     // farther
        }

        public override string Handle(DecoratedTestQuery query)
        {
            return $"*** {query.Text} ***";
        }
    }

    [Decorator]
    internal class DivisionByZeroLoggingQueryHandler : QueryHandlerBase<DivisionByZeroQuery, int>
    {
        private readonly ILogger logger;

        public DivisionByZeroLoggingQueryHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override int Handle(DivisionByZeroQuery query)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = this.next.Handle(query);
            this.logger.LogInformation($"<<<{this.Name}");
            return result;
        }
    }

    [Decorator]
    internal class DivisionByZeroExceptionQueryHandler : QueryHandlerBase<DivisionByZeroQuery, int>
    {
        private readonly ILogger logger;
        protected IEnumerable<Type> ExceptionTypes { get; }

        public DivisionByZeroExceptionQueryHandler(IEnumerable<Type> exceptionTypes, ILogger logger)
        {
            this.ExceptionTypes = exceptionTypes;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override int Handle(DivisionByZeroQuery query)
        {
            this.logger.LogInformation($">>>{this.Name}");

            var result = default(int);
            try
            {
                result = this.next.Handle(query);
            }
            catch (/*DivideByZero*/Exception x)
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

    internal class DivisionByZeroQueryHandler : QueryHandlerBase<DivisionByZeroQuery, int>
    {
        public DivisionByZeroQueryHandler(ILogger logger)
        {
            this.Add(new DivisionByZeroExceptionQueryHandler(new[] { typeof(DivideByZeroException) }, logger));   // closer
            this.Add(new DivisionByZeroLoggingQueryHandler(logger));     // farther
        }

        public override int Handle(DivisionByZeroQuery query) => 123 / query.Zero;    // throws DivideByZeroException
    }
}