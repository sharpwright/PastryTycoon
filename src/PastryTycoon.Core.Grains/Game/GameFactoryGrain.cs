using System;
using Microsoft.Extensions.Logging;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Game.Validators;
using FluentValidation;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Factory grain responsible for creating new game instances.
/// </summary>
public class GameFactoryGrain : Grain, IGameFactoryGrain
{
    private readonly ILogger<GameFactoryGrain> logger;
    private readonly IValidator<CreateNewGameCmd> validator;
    private readonly IRecipeRepository recipeRepository;
    private readonly IGuidProvider guidProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameFactoryGrain"/> class.
    /// </summary>
    public GameFactoryGrain(ILogger<GameFactoryGrain> logger,
        IValidator<CreateNewGameCmd> validator,
        IRecipeRepository recipeRepository,
        IGuidProvider guidProvider)
    {
        this.logger = logger;
        this.validator = validator;
        this.recipeRepository = recipeRepository;
        this.guidProvider = guidProvider;
    }

    /// <summary>
    /// Creates a new game and initializes the game and player grains based on the provided command.
    /// </summary>
    /// <param name="createNewGameCommand">The command containing the details for creating a new game.</param>
    /// <returns>A <see cref="CommandResult"/> indicating the success or failure of the operation.</returns>
    public async Task<CommandResult> CreateNewGameAsync(CreateNewGameCmd createNewGameCommand)
    {
        var results = await validator.ValidateAsync(createNewGameCommand);

        if (!results.IsValid)
        {
            return CommandResult.Failure([.. results.Errors.Select(e => e.ErrorMessage)]);
        }

        // Generate a new game ID and get the game grain.
        var gameId = guidProvider.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);

        // TODO: initialize the player and/or game grains based on the specified difficulty level.
        // Initialize the player grain.
        var playerId = createNewGameCommand.PlayerId;
        var player = GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var initPlayerCmd = new InitPlayerCmd(Guid.NewGuid(), createNewGameCommand.PlayerName, gameId);
        var initPlayerResult = await player.InitializeAsync(initPlayerCmd);

        if (!initPlayerResult.IsSuccess)
        {
            logger.LogError("Failed to initialize player grain for PlayerId: {PlayerId}. Errors: {Errors}",
                playerId, initPlayerResult.Errors);

            return CommandResult.Failure([.. initPlayerResult.Errors]);
        }

        // Populate discoverable recipes.
        var recipes = await recipeRepository.GetAllRecipesAsync();
        var recipeIds = recipes.Select(r => r.Id).ToList();

        // Initialize the game grain.
        var initGameCmd = new InitGameStateCmd(gameId, playerId, recipeIds, DateTime.UtcNow);
        var initGameResult = await gameGrain.InitializeGameStateAsync(initGameCmd);

        if (!initGameResult.IsSuccess)
        {
            // TODO: Clean up the player grain if game initialization fails.
            // For now, we will just log the error and return a failure result.
            logger.LogError("Failed to initialize game state for GameId: {GameId}, PlayerId: {PlayerId}. Errors: {Errors}",
                gameId, playerId, initGameResult.Errors);

            return CommandResult.Failure([.. initGameResult.Errors]);
        }

        return CommandResult.Success();
    }
}
