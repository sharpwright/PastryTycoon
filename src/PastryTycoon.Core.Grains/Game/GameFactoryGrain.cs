using System;
using Microsoft.Extensions.Logging;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Game.Validators;
using FluentValidation;

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

    /// <summary>
    /// Creates a new game and initializes the game and player grains based on the provided command.
    /// </summary>
    /// <param name="createNewGameCommand">The command containing the details for creating a new game.</param>
    /// <returns></returns>
    public async Task<Guid> CreateNewGameAsync(CreateNewGameCommand createNewGameCommand)
    {
        var validator = new CreateNewGameCommandValidator();
        await validator.ValidateCommandAndThrowsAsync(createNewGameCommand, new object(), Guid.Empty);

        var gameId = guidProvider.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);

        // TODO: initialize the player and/or game grains based on the specified difficulty level.

        // Initialize the player grain.
        var playerId = createNewGameCommand.PlayerId;
        var player = GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var initializePlayerCommand = new InitializePlayerCommand(createNewGameCommand.PlayerName, gameId);
        await player.InitializeAsync(initializePlayerCommand);

        // Populate discoverable recipes.
        var recipes = await recipeRepository.GetAllRecipesAsync();
        var recipeIds = recipes.Select(r => r.Id).ToList();

        // Initialize the game grain.
        var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
        await gameGrain.InitializeGameStateAsync(command);

        return gameId;
    }
}
