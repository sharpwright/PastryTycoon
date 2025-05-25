using System;
using BakerySim.Common.Actors;
using BakerySim.Common.Commands;

namespace BakerySim.Grains.Actors;

public class GameFactoryGrain : Grain, IGameFactoryGrain
{
    public async Task<Guid> CreateNewGameAsync(Guid playerId, string gameName)
    {
        var gameId = Guid.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);

        // Populate with standard recipes
        foreach (var recipe in DiscoverableRecipes.StandardRecipes)
        {
            var command = new AddRecipeToGameCommand(gameId, recipe.RecipeId);
            await gameGrain.AddAvailableRecipeAsync(command);
        }

        // Start the game
        await gameGrain.StartGame(new StartGameCommand(gameId, playerId, gameName, DateTime.UtcNow));

        return gameId;
    }

    /// <summary>
    /// Represents a recipe in the bakery simulation game.
    /// TODO: Move this to a shared library or a more appropriate location.
    /// </summary>
    private class Recipe
    {
        public Guid RecipeId { get; set; }
        public required string Name { get; set; }
    }

    /// <summary>
    /// Contains a collection of discoverable recipes for the bakery simulation game.
    /// TODO: Move this to a shared library or a more appropriate location.
    /// </summary>
    private static class DiscoverableRecipes
    {
        public static IEnumerable<Recipe> StandardRecipes { get; } = new List<Recipe>
        {
            new Recipe { RecipeId = Guid.NewGuid(), Name = "Sourdough Bread" },
            new Recipe { RecipeId = Guid.NewGuid(), Name = "Chocolate Cake" },
            new Recipe { RecipeId = Guid.NewGuid(), Name = "Croissant" },
            new Recipe { RecipeId = Guid.NewGuid(), Name = "Bagel" },
            new Recipe { RecipeId = Guid.NewGuid(), Name = "Blueberry Muffin" }
        };
    }
}
