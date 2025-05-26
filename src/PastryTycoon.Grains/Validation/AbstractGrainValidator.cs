using System;
using System.Net.NetworkInformation;
using FluentValidation;
using Orleans.Serialization.Codecs;

namespace PastryTycoon.Grains.Validation;

public abstract class AbstractGrainValidator<TCommand, TState, TPrimaryKey> : AbstractValidator<GrainValidationContext<TCommand, TState, TPrimaryKey>>
{
    /// <summary>
    /// Validates the command against the grain state and primary key. Throws an <see cref="ArgumentExcepion"/> if validation fails.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="grainState"></param>
    /// <param name="grainPrimaryKey"></param>
    /// <throws cref="ArgumentException">
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public virtual async Task ValidateCommandAsync(
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
    }
}
