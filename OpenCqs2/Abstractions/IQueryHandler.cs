namespace OpenCqs2.Abstractions
{
    public interface IQueryHandler<TQ, TR> : IHandler where TQ : IQuery
    {
        HandlerResult<TR> Handle(TQ query);
    }
}