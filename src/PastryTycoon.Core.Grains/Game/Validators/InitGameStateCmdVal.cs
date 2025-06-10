using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game.Validators;

/// <summary>
/// Validator for the InitializeGameStateCommand.
/// </summary>
public class InitGameStateCmdVal : AbstractValidator<InitGameStateCmd>
{
    public InitGameStateCmdVal()
    {
        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("GameId is required");

        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");

        RuleFor(x => x.RecipeIds)
            .NotEmpty()
            .WithMessage("RecipeIds are required")
            .NotNull()
            .WithMessage("RecipeIds cannot be null");

        RuleFor(x => x.StartTimeUtc)
            .NotEmpty()
            .WithMessage("StartTimeUtc is required")
            .Must(startTime => startTime <= DateTime.UtcNow)
            .WithMessage("StartTimeUtc must be in the past or present");
    }

}
