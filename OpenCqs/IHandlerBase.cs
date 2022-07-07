/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using System.Threading.Tasks;

namespace OpenCqs
{
    /// <summary>
    /// The query handler.
    /// </summary>
    /// <typeparam name="T">The query type.</typeparam>
    /// <typeparam name="TR">The return type.</typeparam>
    public interface IQueryHandler<T, TR> where T : IQuery
    {
        /// <summary>
        /// Handles the query.
        /// </summary>
        /// <param name="arg">The query.</param>
        /// <returns>A TR.</returns>
        TR Handle(T arg);
    }

    /// <summary>
    /// The query async handler.
    /// </summary>
    /// <typeparam name="T">The query type.</typeparam>
    /// <typeparam name="TR">The return type.</typeparam>
    public interface IQueryHandlerAsync<T, TR> where T : IQuery
    {
        /// <summary>
        /// Handles the query asynchronously.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A Task.</returns>
        Task<TR> HandleAsync(T arg);
    }

    /// <summary>
    /// The command handler.
    /// </summary>
    /// <typeparam name="T">The command type.</typeparam>
    /// <typeparam name="TR">The return type. OpenCqs commands return value usually indicating whether the command has succeeded (see the examples in the demo app).</typeparam>
    public interface ICommandHandler<T, TR> where T : ICommand
    {
        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="arg">The command.</param>
        /// <returns>A TR (ComandResult in demo app).</returns>
        TR Handle(T arg);
    }

    /// <summary>
    /// The command async handler.
    /// </summary>
    /// <typeparam name="T">The command type.</typeparam>
    /// <typeparam name="TR">The return type. OpenCqs commands return value usually indicating whether the command has succeeded (see the examples in the demo app).</typeparam>
    public interface ICommandHandlerAsync<T, TR> where T : ICommand
    {
        /// <summary>
        /// Handles the command asynchronously.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A Task.</returns>
        Task<TR> HandleAsync(T arg);
    }
}