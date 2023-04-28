namespace OpenCqs2.Abstractions
{
    public interface IQueryHandlerAsync<TQ, TR> where TQ : IQuery
    {
        Task<HandlerResult<TR>> HandleAsync(TQ query);
    }
}