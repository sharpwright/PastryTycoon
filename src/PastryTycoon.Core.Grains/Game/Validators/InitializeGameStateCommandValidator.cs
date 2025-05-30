using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

/// <summary>
/// Validator for the InitializeGameStateCommand.
/// </summary>
public class InitializeGameStateCommandValidator : AbstractGrainValidator<InitializeGameStateCommand, GameState, Guid>
{
    public InitializeGameStateCommandValidator()
    {
        RuleFor(x => x.Command.GameId)
            .NotEmpty()
            .WithMessage("GameId is required")
            .Must((context, gameId) => gameId == context.GrainPrimaryKey)
            .WithMessage("GameId must match grain primary key");

        RuleFor(x => x.Command.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");

        RuleFor(x => x.Command.RecipeIds)
            .NotNull()
            .WithMessage("RecipeIds cannot be null");

        RuleFor(x => x.Command.StartTimeUtc)
            .NotEmpty()
            .WithMessage("StartTimeUtc is required");
    }

}
