using System;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Player.Validators;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Core.Grains.Player.CommandHandlers;

/// <summary>
/// Handles recipe discovery commands and produces discovery events when appropriate.
/// </summary>
public class TryDiscoverRecipeCmdHdlr : ICommandHandler<TryDiscoverRecipeCmd, PlayerState, PlayerEvent>
{
    private readonly IRecipeRepository recipeRepository;
    private readonly IValidator<TryDiscoverRecipeCmd> validator;

    public TryDiscoverRecipeCmdHdlr(
        IRecipeRepository recipeRepository,
        IValidator<TryDiscoverRecipeCmd> validator)
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
    public async Task<CommandHandlerResult<PlayerEvent>> HandleAsync(TryDiscoverRecipeCmd command, PlayerState state, string grainId)
    {
        // Validate the command        
        var results = await validator.ValidateAsync(command);

        if (!results.IsValid)
        {
            // If validation fails, return a failure result with errors
            return CommandHandlerResult<PlayerEvent>.Failure(results.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        // Look up the recipe based on the provided ingredient IDs
        var recipe = await recipeRepository.GetRecipeByIngredientIdsAsync(command.IngredientIds);

        // Perform state checks and raise an event if a new recipe is discovered
        // Check if the recipe exists and if it has not been discovered by the player
        var shouldDiscover = recipe != null &&
                Guid.Parse(grainId) == command.PlayerId &&
                state.PlayerId == command.PlayerId &&
                !state.DiscoveredRecipes.ContainsKey(recipe.Id);

        if (!shouldDiscover)
        {
            // Recipe does not need to be discoverd, return success without an event.
            return CommandHandlerResult<PlayerEvent>.Success();
        }

        // Recipe exists and has not been discovered, so we can raise the discovery event.
        return CommandHandlerResult<PlayerEvent>.Success(
            new PlayerDiscoveredRecipeEvent(
                command.PlayerId,
                recipe!.Id,
                DateTime.UtcNow));
    }
}