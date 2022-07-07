/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.Logging;

using OpenCqs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCqsDemo.Commands
{
    internal class CommandHandlerAsync : CommandHandlerBaseAsync<TestCommandAsync, CommandResult>
    {
        public override async Task<CommandResult> HandleAsync(TestCommandAsync command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return await Task.FromResult(CommandResult.Empty);
        }
    }

    internal class CommandWithValueHandlerAsync : CommandHandlerBaseAsync<TestWithValueCommandAsync, CommandResult>
    {
        private readonly IValueProvider valueProvider;

        public CommandWithValueHandlerAsync(IValueProvider valueProvider)
        {
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
        }

        public override async Task<CommandResult> HandleAsync(TestWithValueCommandAsync command)
        {
            var result = command.Value + this.valueProvider.Value;
            Console.WriteLine($"*** {result.ToString()} ***");
            return await Task.FromResult(CommandResult.Empty);
        }
    }

    [Decorator]
    internal class LoggingCommandHandlerAsync : CommandHandlerBaseAsync<DecoratedTestCommandAsync, CommandResult>
    {
        public override async Task<CommandResult> HandleAsync(DecoratedTestCommandAsync command)
        {
            Console.WriteLine($">>>{this.Name}");
            await this.next?.HandleAsync(command);
            Console.WriteLine($"<<<{this.Name}");
            return CommandResult.Empty;
        }
    }

    [Decorator]
    internal class ExceptionCommandHandlerAsync : CommandHandlerBaseAsync<DecoratedTestCommandAsync, CommandResult>
    {
        public override async Task<CommandResult> HandleAsync(DecoratedTestCommandAsync command)
        {
            Console.WriteLine($">>>{this.Name}");
            await this.next?.HandleAsync(command);
            Console.WriteLine($"<<<{this.Name}");
            return CommandResult.Empty;
        }
    }

    internal class DecoratedCommandHandlerAsync : CommandHandlerBaseAsync<DecoratedTestCommandAsync, CommandResult>
    {
        public DecoratedCommandHandlerAsync()
        {
            this.Add(new ExceptionCommandHandlerAsync());   // closer
            this.Add(new LoggingCommandHandlerAsync());     // farther
        }

        public override async Task<CommandResult> HandleAsync(DecoratedTestCommandAsync command)
        {
            Console.WriteLine($"*** {command.Arg} ***");
            return await Task.FromResult(CommandResult.Empty);
        }
    }

    [Decorator]
    internal class DivisionByZeroLoggingCommandHandlerAsync : CommandHandlerBaseAsync<DivisionByZeroCommandAsync, CommandResult>
    {
        private readonly ILogger logger;

        public DivisionByZeroLoggingCommandHandlerAsync(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CommandResult> HandleAsync(DivisionByZeroCommandAsync command)
        {
            this.logger.LogInformation($">>>{this.Name}");
            var result = await this.next.HandleAsync(command);
            this.logger.LogInformation($"<<<{this.Name}");
            return result;
        }
    }

    [Decorator]
    internal class DivisionByZeroExceptionCommandHandlerAsync : CommandHandlerBaseAsync<DivisionByZeroCommandAsync, CommandResult>
    {
        private readonly ILogger logger;
        protected IEnumerable<Type> ExceptionTypes { get; }

        public DivisionByZeroExceptionCommandHandlerAsync(IEnumerable<Type> exceptionTypes, ILogger logger)
        {
            this.ExceptionTypes = exceptionTypes;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CommandResult> HandleAsync(DivisionByZeroCommandAsync Command)
        {
            this.logger.LogInformation($">>>{this.Name}");

            var result = default(CommandResult);
            try
            {
                result = await this.next.HandleAsync(Command);
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

    internal class DivisionByZeroCommandHandlerAsync : CommandHandlerBaseAsync<DivisionByZeroCommandAsync, CommandResult>
    {
        public DivisionByZeroCommandHandlerAsync(ILogger logger)
        {
            this.Add(new DivisionByZeroExceptionCommandHandlerAsync(new[] { typeof(DivideByZeroException) }, logger));   // closer
            this.Add(new DivisionByZeroLoggingCommandHandlerAsync(logger));     // farther
        }

        public override async Task<CommandResult> HandleAsync(DivisionByZeroCommandAsync Command)
        {
            _ = await Task.FromResult(123 / Command.Zero);
            return await Task.FromResult(CommandResult.Empty);
        } 
    }
}