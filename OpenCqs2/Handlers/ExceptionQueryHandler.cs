using ExplicitDecorators.Abstractions;

namespace ExplicitDecorators.Handlers
{
    internal class ExceptionQueryHandler<TQ, TR> : IQueryHandler<TQ, TR> where TQ : IQuery
    {
        private readonly IQueryHandler<TQ, TR> decorated;

        public ExceptionQueryHandler(IQueryHandler<TQ, TR> decorated, IEnumerable<Type>? exceptionTypes = default)
        {
            this.decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            this.ExceptionTypes = exceptionTypes;
        }

        public IEnumerable<Type>? ExceptionTypes { get; set; }

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
            var result = default(HandlerResult<TR>);
            try
            {
                result = this.decorated.Handle(query);
            }
            catch (Exception x) when (this.HandleException(x))
            {
            }

            //this.logger.LogInformation($"<<<{this.Name}");

            return result ?? new HandlerResult<TR>(HandlerResults.Fail);
        }

        protected bool HandleException(Exception ex)
        {
            if (this.ExceptionTypes == default || this.ExceptionTypes.Any(x => x == ex.GetType()))
            {
                //this.logger.LogError(ex, string.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}