using System;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.TestKit;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Abstractions.Player;
using FluentValidation;
using FluentValidation.Results;
using PastryTycoon.Core.Abstractions.Common;

namespace PastryTycoon.Core.Grains.UnitTests.Game;

/// <summary>
/// Tests for the GameFactoryGrain actor using Orleans TestKit.
/// </summary>
public class GameFactoryGrainTests : TestKitBase
{
    private readonly Mock<IRecipeRepository> recipeRepositoryMock = new();
    private readonly Mock<IValidator<CreateNewGameCmd>> validatorMock = new();
    private readonly Mock<IGuidProvider> guidProviderMock = new();

    private readonly Guid gameId = Guid.NewGuid();
    private readonly Guid playerId = Guid.NewGuid();
    private readonly string playerName = "Test Player";
    private readonly List<Recipe> recipes = new()
    {
        new Recipe("test-1-recipe", "Test 1", new List<RecipeIngredient>()),
        new Recipe("test-2-recipe", "Test 2", new List<RecipeIngredient>(), ProducesIngredientId: "test-2")
    };

    public GameFactoryGrainTests()
    {
        // Setup the mock for the recipe repository to return a predefined list of recipes.
        recipeRepositoryMock
            .Setup(r => r.GetAllRecipesAsync())
            .ReturnsAsync(recipes);

        // Setup the mock for the guid provider to return a specific game ID.
        guidProviderMock
            .Setup(g => g.NewGuid())
            .Returns(gameId);

        // Setup the validator to always return valid results.
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateNewGameCmd>(), default))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task CreateNewGameAsync_ShouldCreateGame_And_ShouldInitializeState()
    {
        // Add all mock objects to the Silo
        Silo.AddService(validatorMock.Object);
        Silo.AddService(guidProviderMock.Object);
        Silo.AddService(recipeRepositoryMock.Object);

        // Add probes to verify interactions with grains
        var playerGrainProbe = Silo.AddProbe<IPlayerGrain>(playerId);
        playerGrainProbe
            .Setup(p => p.InitializeAsync(It.IsAny<InitPlayerCmd>()))
            .Returns(Task.FromResult(CommandResult.Success()));

        var gameGrainProbe = Silo.AddProbe<IGameGrain>(gameId);
        gameGrainProbe
            .Setup(g => g.InitializeGameStateAsync(It.IsAny<InitGameStateCmd>()))
            .Returns(Task.FromResult(CommandResult.Success()));

        // Act
        var grain = await Silo.CreateGrainAsync<GameFactoryGrain>(Guid.Empty);
        var createNewGameCommand = new CreateNewGameCmd(playerId, playerName, DifficultyLevel.Easy);
        var result = await grain.CreateNewGameAsync(createNewGameCommand);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess, "Game creation should be successful.");
        Assert.Empty(result.Errors);

        playerGrainProbe.Verify(
            p => p.InitializeAsync(It.Is<InitPlayerCmd>(cmd =>
                cmd.PlayerName == playerName &&
                cmd.GameId == gameId
            )),
            Times.Once
        );

        gameGrainProbe.Verify(
            g => g.InitializeGameStateAsync(It.Is<InitGameStateCmd>(cmd =>
                cmd.GameId == gameId &&
                cmd.PlayerId == playerId &&
                cmd.RecipeIds.SequenceEqual(recipes.Select(r => r.Id))
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateNewGameAsync_ShouldReturnFailure_WhenPlayerGrainInitializationFails()
    {
        // Arrange
        Silo.AddService(validatorMock.Object);
        Silo.AddService(guidProviderMock.Object);
        Silo.AddService(recipeRepositoryMock.Object);

        var playerGrainProbe = Silo.AddProbe<IPlayerGrain>(playerId);
        playerGrainProbe
            .Setup(p => p.InitializeAsync(It.IsAny<InitPlayerCmd>()))
            .Returns(Task.FromResult(CommandResult.Failure("Failed to initialize player.")));

        // Act
        var grain = await Silo.CreateGrainAsync<GameFactoryGrain>(Guid.Empty);
        var createNewGameCommand = new CreateNewGameCmd(playerId, playerName, DifficultyLevel.Easy);
        var result = await grain.CreateNewGameAsync(createNewGameCommand);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess, "Game creation should fail due to player grain initialization failure.");
        Assert.Single(result.Errors);
        Assert.Equal("Failed to initialize player.", result.Errors.First());
    }

    [Fact]
    public async Task CreateNewGameAsync_ShouldReturnFailure_WhenGameGrainInitializationFails()
    {
        // Arrange
        Silo.AddService(validatorMock.Object);
        Silo.AddService(guidProviderMock.Object);
        Silo.AddService(recipeRepositoryMock.Object);

        var playerGrainProbe = Silo.AddProbe<IPlayerGrain>(playerId);
        playerGrainProbe
            .Setup(p => p.InitializeAsync(It.IsAny<InitPlayerCmd>()))
            .Returns(Task.FromResult(CommandResult.Success()));

        var gameGrainProbe = Silo.AddProbe<IGameGrain>(gameId);
        gameGrainProbe
            .Setup(g => g.InitializeGameStateAsync(It.IsAny<InitGameStateCmd>()))
            .Returns(Task.FromResult(CommandResult.Failure("Failed to initialize game state.")));

        // Act
        var grain = await Silo.CreateGrainAsync<GameFactoryGrain>(Guid.Empty);
        var createNewGameCommand = new CreateNewGameCmd(playerId, playerName, DifficultyLevel.Easy);
        var result = await grain.CreateNewGameAsync(createNewGameCommand);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess, "Game creation should fail due to game grain initialization failure.");
        Assert.Single(result.Errors);
        Assert.Equal("Failed to initialize game state.", result.Errors.First());
    }

    /// <summary>
    /// Example of a unit test verifying an event is pushed on to a stream.
    /// </summary>
    /// <returns></returns>
    // [Fact]
    // public async Task StartGame_Should_Send_GameStartedEvent()
    // {
    //     // Arrange
    //     var gameId = Guid.NewGuid();
    //     var command = new StartGameCommand(gameId, "Test Game", DateTime.UtcNow);
    //     var stream = Silo.AddStreamProbe<GameStartedEvent>(
    //         gameId, OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
    //     var grain = await Silo.CreateGrainAsync<GameGrain>(gameId);

    //     // Act
    //     await grain.StartGame(command);

    //     // Assert
    //     stream.VerifySend(evt => evt.GameId == gameId, Times.Once());
    // }
}
