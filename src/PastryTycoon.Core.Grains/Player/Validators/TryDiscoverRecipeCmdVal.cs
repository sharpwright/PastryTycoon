using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

/// <summary>
/// Validator for the DiscoverRecipeCommand.
/// </summary>
public class TryDiscoverRecipeCmdVal : AbstractValidator<TryDiscoverRecipeCmd>
{
    public TryDiscoverRecipeCmdVal()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required");    

        RuleFor(x => x.IngredientIds)
            .Must(ingredientIds => ingredientIds != null && ingredientIds.Count > 0)
            .WithMessage("At least one ingredient ID must be provided");
    }
}
