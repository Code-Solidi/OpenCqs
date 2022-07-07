/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using OpenCqs;

namespace OpenCqsDemo.Commands
{
    internal class CommandResult
    {
        private static CommandResult empty;

        public static CommandResult Empty => CommandResult.empty ?? (CommandResult.empty = new CommandResult());
    }

    internal class TestCommand : ICommand
    {
        public string Arg { get; set; }
    }

    internal class TestCommandAsync : ICommand
    {
        public string Arg { get; set; }
    }

    internal class DecoratedTestCommand : ICommand
    {
        public string Arg { get; set; }
    }

    internal class DecoratedTestCommandAsync : ICommand
    {
        public string Arg { get; set; }
    }

    internal class TestWithValueCommand : ICommand
    {
        public int Value { get; set; }
    }

    internal class TestWithValueCommandAsync : ICommand
    {
        public int Value { get; set; }
    }

    internal class DivisionByZeroCommand : ICommand
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }

    internal class DivisionByZeroCommandAsync : ICommand
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }
}