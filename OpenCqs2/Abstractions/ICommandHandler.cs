namespace OpenCqs2.Abstractions
{
    public interface ICommandHandler<TC, TR> : IHandler where TC : ICommand
    {
        HandlerResult<TR> Handle(TC command);
    }
}