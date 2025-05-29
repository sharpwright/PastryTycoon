using System;
using Microsoft.Extensions.Logging;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Common;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Factory grain responsible for creating new game instances.
/// </summary>
public class GameFactoryGrain : Grain, IGameFactoryGrain
{
    private readonly ILogger<GameFactoryGrain> logger;
    private readonly IRecipeRepository recipeRepository;
    private readonly IGuidProvider guidProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameFactoryGrain"/> class.
    /// </summary>
    public GameFactoryGrain(ILogger<GameFactoryGrain> logger,
        IRecipeRepository recipeRepository,
        IGuidProvider guidProvider)
    {
        this.logger = logger;
        this.recipeRepository = recipeRepository;
        this.guidProvider = guidProvider;
    }

    public async Task<Guid> CreateNewGameAsync(Guid playerId, string gameName)
    {
        var gameId = guidProvider.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);

        // Populate discoverable recipes.
        var recipes = await recipeRepository.GetAllRecipesAsync();
        var recipeIds = recipes.Select(r => r.Id).ToList();

        // Start the game
        var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, gameName, DateTime.UtcNow);
        await gameGrain.InitializeGameStateAsync(command);

        return gameId;
    }
}
