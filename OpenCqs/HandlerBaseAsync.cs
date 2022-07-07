/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */


using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenCqs
{
    /// <summary>
    /// The handler base async.
    /// </summary>
    /// <typeparam name="T">The command/query type.</typeparam>
    /// <typeparam name="TR">The return type.</typeparam>
    public abstract class HandlerBaseAsync<T, TR> where T : ICqsBase
    {
        protected internal HandlerBaseAsync<T, TR> next;

        /// <summary>
        /// Gets the last handler in the chain.
        /// </summary>
        private HandlerBaseAsync<T, TR> last => this.next?.last ?? this;    // may cause issues when debugging (see Build())

        /// <summary>
        /// Gets the handler name.
        /// </summary>
        protected string Name => this.GetType().Name;

        /// <summary>
        /// Handles the command/query asynchronously.
        /// </summary>
        /// <param name="arg">The arg, command or query.</param>
        /// <returns>A TR.</returns>
        public abstract Task<TR> HandleAsync(T arg);

        /// <summary>
        /// Builds the handler chain.
        /// </summary>
        /// <returns>A HandlerBase.</returns>
        [DebuggerStepThrough]
        internal HandlerBaseAsync<T, TR> Build()
        {
            // start with next
            var head = this.next;

            // determine last in the chain and link to this
            this.last.next = this;

            // this is the show-stopper
            this.next = null;

            return this.CheckDecorators(head ?? this);
        }

        /// <summary>
        /// Checks the decorators.
        /// </summary>
        /// <param name="item">The head decorator.</param>
        /// <returns>Returns the new head, a HandlerBase.</returns>
        private HandlerBaseAsync<T, TR> CheckDecorators(HandlerBaseAsync<T, TR> item)
        {
            for (var current = item; current != this; current = current.next)
            {
                var type = current.GetType();
                if (type.GetCustomAttributes(typeof(DecoratorAttribute), false) == default)
                {
                    throw new InvalidOperationException($"In order to use '{type.Name}' as decorating handler add [Decorator] attribute to it.");
                }
            }

            return item;
        }

        /// <summary>
        /// Adds the decorator to the chain.
        /// </summary>
        /// <param name="next">The decorator.</param>
        public void Add(HandlerBaseAsync<T, TR> next)
        {
            _ = next ?? throw new ArgumentNullException(nameof(next));
            if (this.next != null)
            {
                next.next = this.next;
            }

            this.next = next;
        }
    }

    /// <summary>
    /// The query async handler base.
    /// </summary>
    public abstract class QueryHandlerBaseAsync<T, TR> : HandlerBaseAsync<T, TR>, IQueryHandlerAsync<T, TR> where T : IQuery
    {
    }

    /// <summary>
    /// The command async handler base.
    /// </summary>
    public abstract class CommandHandlerBaseAsync<T, TR> : HandlerBaseAsync<T, TR>, ICommandHandlerAsync<T, TR> where T : ICommand
    {
    }
}