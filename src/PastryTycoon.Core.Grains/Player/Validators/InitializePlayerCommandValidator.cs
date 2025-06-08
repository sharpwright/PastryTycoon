using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the InitializePlayerCommand.
/// </summary>
public class InitializePlayerCommandValidator : AbstractGrainValidator<InitializePlayerCommand, PlayerState, Guid>
{
    public InitializePlayerCommandValidator()
    {
        RuleFor(x => x.GrainState.IsInitialized)
            .NotEqual(true)
            .WithMessage("Player is already initialized");

        RuleFor(x => x.Command.PlayerName)
            .NotEmpty()
            .WithMessage("PlayerName is required")
            .MaximumLength(50)
            .WithMessage("PlayerName cannot exceed 50 characters");

        RuleFor(x => x.Command.GameId)
            .NotEmpty()
            .WithMessage("GameId is required");
    }
}
