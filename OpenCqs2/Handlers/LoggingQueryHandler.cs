using ExplicitDecorators.Abstractions;

using Microsoft.Extensions.Logging;

namespace ExplicitDecorators.Handlers
{
    internal class LoggingQueryHandler<TQ, TR> : IQueryHandler<TQ, TR> where TQ : IQuery
    {
        private readonly ILogger<LoggingQueryHandler<TQ, TR>> logger;
        private readonly IQueryHandler<TQ, TR> decorated;
        private readonly string before;
        private readonly string after;

        public LoggingQueryHandler(IQueryHandler<TQ, TR> decorated, ILoggerFactory loggerFactory, string before, string after)
        {
            if (string.IsNullOrEmpty(before))
            {
                throw new ArgumentException($"'{nameof(before)}' cannot be null or empty.", nameof(before));
            }

            this.before = before;

            if (string.IsNullOrEmpty(after))
            {
                throw new ArgumentException($"'{nameof(after)}' cannot be null or empty.", nameof(after));
            }

            this.after = after;

            this.decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.logger = loggerFactory.CreateLogger<LoggingQueryHandler<TQ, TR>>();
        }

        public IHandler? Decorator { get; set; }

        public Type GetContract()
        {
            throw new NotImplementedException();
        }

        public Type GetImplemetnation()
        {
            throw new NotImplementedException();
        }

        public HandlerResult<TR> Handle(TQ query)
        {
            this.logger.LogInformation(this.before);
            var result = this.decorated.Handle(query);
            this.logger.LogInformation(this.after, result);
            return result;
        }
    }
}