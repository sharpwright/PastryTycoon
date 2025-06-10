using System;
using FluentValidation.Results;

namespace PastryTycoon.Core.Grains.Common;

public interface IGrainValidator<TCommand, TState, TPrimaryKey>
{
    Task<ValidationResult> ValidateCommandAsync(TCommand command, TState grainState, TPrimaryKey grainPrimaryKey);
}
