/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using OpenCqs2.Abstractions;

namespace OpenCqs2Demo.Commands
{
    /*internal class CommandResult
    {
        private static CommandResult empty;

        public static CommandResult Empty => empty ?? (empty = new CommandResult());
    }*/

    public class TestCommand : ICommand
    {
        public string Arg { get; set; } = null!;
    }

    public class TestCommandAsync : ICommand
    {
        public string Arg { get; set; } = null!;
    }

    public class DecoratedTestCommand : ICommand
    {
        public string Arg { get; set; } = null!;
    }

    public class DecoratedTestCommandAsync : ICommand
    {
        public string Arg { get; set; } = null!;
    }

    public class TestWithValueCommand : ICommand
    {
        public int Value { get; set; }
    }

    public class TestWithValueCommandAsync : ICommand
    {
        public int Value { get; set; }
    }

    public class DivisionByZeroCommand : ICommand
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }

    public class DivisionByZeroCommandAsync : ICommand
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }
}