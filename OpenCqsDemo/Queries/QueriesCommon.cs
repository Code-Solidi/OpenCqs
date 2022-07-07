/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */


using OpenCqs;

namespace OpenCqsDemo.Queries
{
    internal class TestQuery : IQuery
    {
        public string Text { get; set; }
    }

    internal class TestQueryAsync : IQuery
    {
        public string Text { get; set; }
    }

    internal class TestWithValueQuery : IQuery
    {
        public int Value { get; set; }
    }

    internal class TestWithValueQueryAsync : IQuery
    {
        public int Value { get; set; }
    }

    internal class DecoratedTestQuery : IQuery
    {
        public string Text { get; set; }
    }

    internal class DecoratedTestQueryAsync : IQuery
    {
        public string Text { get; set; }
    }

    internal class DivisionByZeroQuery : IQuery
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }

    internal class DivisionByZeroQueryAsync : IQuery
    {
        public int Zero => 0;

        public override string ToString() => $"{this.GetType().FullName}: ({nameof(this.Zero)}: {this.Zero})";
    }
}