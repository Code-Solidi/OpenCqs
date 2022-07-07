/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.Logging;

using OpenCqs;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCqsDemo.Commands
{
    internal class CommandHandler : CommandHandlerBase<TestCommand, CommandResult>
    {
        public override CommandResult Handle(TestCommand command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return CommandResult.Empty;
        }
    }

    internal class CommandWithValueHandler : CommandHandlerBase<TestWithValueCommand, CommandResult>
    {
        private readonly IValueProvider valueProvider;

        public CommandWithValueHandler(IValueProvider valueProvider)
        {
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
        }

        public override CommandResult Handle(TestWithValueCommand command)
        {
            var result = command.Value + this.valueProvider.Value;
            Console.WriteLine($"*** {result.ToString()} ***");
            return CommandResult.Empty;
        }
    }

    [Decorator]
    internal class LoggingCommandHandler : CommandHandlerBase<DecoratedTestCommand, CommandResult>
    {
        public override CommandResult Handle(DecoratedTestCommand command)
        {
            Console.WriteLine($">>>{this.Name}");
            this.next?.Handle(command);
            Console.WriteLine($"<<<{this.Name}");
            return CommandResult.Empty;
        }
    }

    [Decorator]
    internal class ExceptionCommandHandler : CommandHandlerBase<DecoratedTestCommand, CommandResult>
    {
        public override CommandResult Handle(DecoratedTestCommand command)
        {
            Console.WriteLine($">>>{this.Name}");
            this.next?.Handle(command);
            Console.WriteLine($"<<<{this.Name}");
            return CommandResult.Empty;
        }
    }

    internal class DecoratedCommandHandler : CommandHandlerBase<DecoratedTestCommand, CommandResult>
    {
        public DecoratedCommandHandler()
        {
            this.Add(new ExceptionCommandHandler());   // closer
            this.Add(new LoggingCommandHandler());     // farther
        }

        public override CommandResult Handle(DecoratedTestCommand command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return CommandResult.Empty;
        }
    }

    [Decorator]
    internal class DivisionByZeroLoggingCommandHandler : CommandHandlerBase<DivisionByZeroCommand, CommandResult>
    {
        private readonly ILogger logger;

        public DivisionByZeroLoggingCommandHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override CommandResult Handle(DivisionByZeroCommand Command)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = this.next.Handle(Command);
            this.logger.LogInformation($"<<<{this.Name}");
            return CommandResult.Empty;
        }
    }

    [Decorator]
    internal class DivisionByZeroExceptionCommandHandler : CommandHandlerBase<DivisionByZeroCommand, CommandResult>
    {
        private readonly ILogger logger;
        protected IEnumerable<Type> ExceptionTypes { get; }

        public DivisionByZeroExceptionCommandHandler(IEnumerable<Type> exceptionTypes, ILogger logger)
        {
            this.ExceptionTypes = exceptionTypes;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override CommandResult Handle(DivisionByZeroCommand Command)
        {
            this.logger.LogInformation($">>>{this.Name}");

            var result = default(CommandResult);
            try
            {
                result = this.next.Handle(Command);
            }
            catch (Exception x)
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

    internal class DivisionByZeroCommandHandler : CommandHandlerBase<DivisionByZeroCommand, CommandResult>
    {
        public DivisionByZeroCommandHandler(ILogger logger)
        {
            this.Add(new DivisionByZeroExceptionCommandHandler(new[] { typeof(DivideByZeroException) }, logger));   // closer
            this.Add(new DivisionByZeroLoggingCommandHandler(logger));     // farther
        }

        public override CommandResult Handle(DivisionByZeroCommand Command)
        {
            _ = 123 / Command.Zero;    // throws DivideByZeroException
            return CommandResult.Empty;
        }
    }
}