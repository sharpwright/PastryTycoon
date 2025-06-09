using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Player.Validators;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Core.Grains.Player.CommandHandlers;

/// <summary>
/// Handles recipe discovery commands and produces discovery events when appropriate.
/// </summary>
public class TryDiscoverRecipeCommandHandler : ICommandHandler<TryDiscoverRecipeCommand, PlayerEvent, PlayerState>
{
    private readonly IRecipeRepository recipeRepository;
    private readonly IGrainValidator<TryDiscoverRecipeCommand, PlayerState, Guid> validator;

    public TryDiscoverRecipeCommandHandler(IRecipeRepository recipeRepository,
        IGrainValidator<TryDiscoverRecipeCommand, PlayerState, Guid> validator)
    {
        this.recipeRepository = recipeRepository;
        this.validator = validator;
    }

    /// <summary>
    /// Handles the recipe discovery command.
    /// </summary>
    /// <param name="command">The command to process</param>
    /// <param name="state">Current player state</param>
    /// <param name="playerId">The player's ID</param>
    /// <returns>A discovery event if a new recipe is discovered, null otherwise</returns>
    public async Task<PlayerEvent?> HandleAsync(TryDiscoverRecipeCommand command, PlayerState state, Guid playerId)
    {
        // Validate the command
        await validator.ValidateCommandAndThrowsAsync(command, state, playerId);

        // Look up the recipe based on the provided ingredient IDs
        var recipe = await recipeRepository.GetRecipeByIngredientIdsAsync(command.IngredientIds);

        // If the recipe is found and not already discovered, create an event
        if (recipe != null && !state.DiscoveredRecipes.ContainsKey(recipe.Id))
        {
            return new PlayerDiscoveredRecipeEvent(
                command.PlayerId,
                recipe.Id,
                DateTime.UtcNow);
        }

        // No event to raise
        return null;
    }
}