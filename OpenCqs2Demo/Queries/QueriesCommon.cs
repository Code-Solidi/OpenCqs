/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */


using OpenCqs2.Abstractions;

namespace OpenCqs2Demo.Queries
{
    // NB: queries which are decorated (use DispatchProxy.Create) to create the wrapper must be public. Otherwise TypeLoadException: Access is denied is thrown.
    public class TestQuery : IQuery
    {
        public string Text { get; set; } = null!;
    }

    public class TestQueryAsync : IQuery
    {
        public string Text { get; set; } = null!;
    }

    public class TestWithValueQuery : IQuery
    {
        public int Value { get; set; }
    }

    public class TestWithValueQueryAsync : IQuery
    {
        public int Value { get; set; }
    }

    public class DecoratedTestQuery : IQuery
    {
        public string Text { get; set; } = null!;
    }

    public class DecoratedTestQueryAsync : IQuery
    {
        public string? Text { get; set; }
    }

    public class DivisionByZeroQuery : IQuery
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }

    public class DivisionByZeroQueryAsync : IQuery
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }
}