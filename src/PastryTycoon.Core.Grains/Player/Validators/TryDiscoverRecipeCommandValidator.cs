using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the DiscoverRecipeCommand.
/// </summary>
public class TryDiscoverRecipeCommandValidator : AbstractGrainValidator<TryDiscoverRecipeCommand, PlayerState, Guid>
{
    public TryDiscoverRecipeCommandValidator()
    {
        RuleFor(x => x.Command.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required")
            .Must((context, playerId) => playerId == context.GrainPrimaryKey)
            .WithMessage("PlayerId must match grain primary key");

        RuleFor(x => x.Command.IngredientIds)
            .Must(ingredientIds => ingredientIds != null && ingredientIds.Count > 0)
            .WithMessage("At least one ingredient ID must be provided");
    }
}
