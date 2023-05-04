namespace OpenCqs2.Abstractions
{
    public interface ICommandHandler<T> : IHandler where T : ICommand
    {
        HandlerResult Handle(T command);
    }
}