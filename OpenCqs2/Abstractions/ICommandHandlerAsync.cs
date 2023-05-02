namespace OpenCqs2.Abstractions
{
    public interface ICommandHandlerAsync<TC, TR> : IHandler where TC : ICommand
    {
        Task<HandlerResult<TR>> HandleAsync(TC command);
    }
}