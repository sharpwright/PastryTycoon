using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Player.Validators;

public class DiscoverRecipeCommandValidator : AbstractGrainValidator<DiscoverRecipeCommand, PlayerState, Guid>
{
    public DiscoverRecipeCommandValidator()
    {
        RuleFor(x => x.Command.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId is required")
            .Must((context, playerId) => playerId == context.GrainPrimaryKey)
            .WithMessage("PlayerId must match grain primary key");

        RuleFor(x => x.Command.RecipeId)
            .NotEmpty()
            .WithMessage("RecipeId is required")
            .Must((context, recipeId) => context.GrainState.DiscoveredRecipeIds.ContainsKey(recipeId) == false)
            .WithMessage("RecipeId must not already be discovered by the player");

        RuleFor(x => x.Command.DiscoveryTimeUtc)
            .NotEmpty()
            .WithMessage("DiscoveryTimeUtc is required")
            .Must(discoveryTime => discoveryTime <= DateTime.UtcNow)
            .WithMessage("DiscoveryTimeUtc must be in the past or present");
        
    }
}
