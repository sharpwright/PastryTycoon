using System;

namespace PastryTycoon.Grains.CommandHandlers;

public interface ICommandHandler<TCommand, TState, TPrimaryKey>
    where TCommand : class
    where TState : class
{
    Task<CommandResult> HandleAsync(TCommand command, TState state, TPrimaryKey grainKey);
}
