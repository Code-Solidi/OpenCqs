using OpenCqs2.Abstractions;
using OpenCqs2.Proxies;

using OpenCqs2Demo.Commands;

namespace OpenCqs2Demo.Handlers
{
    public class CommandHandler : ICommandHandler<TestCommand>
    {
        HandlerResult ICommandHandler<TestCommand>.Handle(TestCommand command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return HandlerResult.OK;
        }
    }

    public class CommandWithValueHandler : ICommandHandler<TestWithValueCommand>
    {
        HandlerResult ICommandHandler<TestWithValueCommand>.Handle(TestWithValueCommand command)
        {
            Console.WriteLine($"*** {command.Value} ***");
            return HandlerResult.OK;
        }
    }

    public class DecoratedTestHandler : ICommandHandler<DecoratedTestCommand>
    {
        internal static ICommandHandler<DecoratedTestCommand> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultLoggingProxy<ICommandHandler<DecoratedTestCommand>>.Create(new DecoratedTestHandler(), new DemoLoggingPolicy(provider))!;
        }

        HandlerResult ICommandHandler<DecoratedTestCommand>.Handle(DecoratedTestCommand command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return HandlerResult.OK;
        }
    }

    public class DivisionByZeroCommandHandler : ICommandHandler<DivisionByZeroCommand>
    {
        internal static ICommandHandler<DivisionByZeroCommand> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultExceptionProxy<ICommandHandler<DivisionByZeroCommand>>.Create(new DivisionByZeroCommandHandler(), new DemoExceptionPolicy(provider))!;
        }

        HandlerResult ICommandHandler<DivisionByZeroCommand>.Handle(DivisionByZeroCommand command)
        {
            return new HandlerResult(HandlerResult.HandlerResults.Fail) { Result = (123 / command.Zero).ToString() };
        }
    }

    public class CommandHandlerAsync : ICommandHandlerAsync<TestCommandAsync>
    {
        async Task<HandlerResult> ICommandHandlerAsync<TestCommandAsync>.HandleAsync(TestCommandAsync command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return await Task.Run(() => HandlerResult.OK);
        }
    }

    public class CommandWithValueHandlerAsync : ICommandHandlerAsync<TestWithValueCommandAsync>
    {
        async Task<HandlerResult> ICommandHandlerAsync<TestWithValueCommandAsync>.HandleAsync(TestWithValueCommandAsync command)
        {
            Console.WriteLine($"*** {command.Value} ***");
            return await Task.Run(() => HandlerResult.OK);
        }
    }

    public class DecoratedTestHandlerAsync : ICommandHandlerAsync<DecoratedTestCommandAsync>
    {
        internal static ICommandHandlerAsync<DecoratedTestCommandAsync> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultLoggingProxyAsync<ICommandHandlerAsync<DecoratedTestCommandAsync>>.Create(new DecoratedTestHandlerAsync(), new DemoLoggingPolicy(provider))!;
        }

        async Task<HandlerResult> ICommandHandlerAsync<DecoratedTestCommandAsync>.HandleAsync(DecoratedTestCommandAsync command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return await Task.Run(() => HandlerResult.OK);
        }
    }

    public class DivisionByZeroCommandHandlerAsync : ICommandHandlerAsync<DivisionByZeroCommandAsync>
    {
        internal static ICommandHandlerAsync<DivisionByZeroCommandAsync> CreateProxy(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return DefaultExceptionProxyAsync<ICommandHandlerAsync<DivisionByZeroCommandAsync>>.Create(new DivisionByZeroCommandHandlerAsync(), new DemoExceptionPolicy(provider))!;
        }

        async Task<HandlerResult> ICommandHandlerAsync<DivisionByZeroCommandAsync>.HandleAsync(DivisionByZeroCommandAsync command)
        {
            return await Task.Run(() => new HandlerResult(HandlerResult.HandlerResults.Fail) { Result = (123 / command.Zero).ToString() });
        }
    }
}