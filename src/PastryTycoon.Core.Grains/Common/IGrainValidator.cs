using System;

namespace PastryTycoon.Core.Grains.Common;

public interface IGrainValidator<TCommand, TState, TPrimaryKey>
{
    Task ValidateCommandAndThrowsAsync(TCommand command, TState grainState, TPrimaryKey grainPrimaryKey);
}
