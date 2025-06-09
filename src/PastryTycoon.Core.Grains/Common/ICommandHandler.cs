using System;

namespace PastryTycoon.Core.Grains.Common;

/// <summary>
/// Generic interface for command handlers that process commands and produce events.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled</typeparam>
/// <typeparam name="TState">The type of state the handler operates against</typeparam>
/// <typeparam name="TPrimaryKey">The type of primary key for the grain</typeparam>
/// <typeparam name="TEvent">The type of event that may be produced</typeparam>
public interface ICommandHandler<TCommand, TState, TPrimaryKey, TEvent>
{
    /// <summary>
    /// Handles the command and produces an event if appropriate.
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="state">The current state</param>
    /// <param name="grainId">The ID of the grain handling the command</param>
    /// <returns>An event if one should be raised, or null if no event</returns>
    Task<TEvent> HandleAsync(TCommand command, TState state, TPrimaryKey grainId);
}
