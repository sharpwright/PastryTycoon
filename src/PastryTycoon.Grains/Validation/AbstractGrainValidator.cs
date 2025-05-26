using System;
using System.Net.NetworkInformation;
using FluentValidation;
using Orleans.Serialization.Codecs;

namespace PastryTycoon.Grains.Validation;

public abstract class AbstractGrainValidator<TCommand, TState, TPrimaryKey> : AbstractValidator<GrainValidationContext<TCommand, TState, TPrimaryKey>>
{
    public virtual async Task<bool> ValidateCommandAsync(
            TCommand command,
            TState grainState,
            TPrimaryKey grainPrimaryKey)
    {
        var context = new GrainValidationContext<TCommand, TState, TPrimaryKey>(
            command,
            grainState,
            grainPrimaryKey
        );

        var validationResult = await base.ValidateAsync(context);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errorMessages);
        }
                
        return true;
    }
}
