namespace OpenCqs2.Abstractions
{
    public interface ICommandHandlerAsync<T> : IHandler where T : ICommand
    {
        Task<HandlerResult> HandleAsync(T command);
    }
}