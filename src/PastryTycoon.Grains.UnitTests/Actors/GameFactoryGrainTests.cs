using System;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.TestKit;
using PastryTycoon.Common.Actors;
using PastryTycoon.Common.Commands;
using PastryTycoon.Data.Recipes;
using PastryTycoon.Grains.Actors;
using PastryTycoon.Grains.Providers;
using PastryTycoon.Grains.States;
using PastryTycoon.Grains.Validation;

namespace PastryTycoon.Grains.UnitTests.Actors;

/// <summary>
/// Tests for the GameFactoryGrain actor using Orleans TestKit.
/// </summary>
public class GameFactoryGrainTests : TestKitBase
{
    private readonly Mock<IRecipeRepository> recipeRepositoryMock = new();
    private readonly Mock<ILogger<GameFactoryGrain>> loggerMock = new();
    private readonly Mock<IGuidProvider> guidProviderMock = new();

    private readonly Guid gameId;
    private readonly Guid playerId;
    private readonly string gameName;
    private readonly List<Recipe> recipes;

    public GameFactoryGrainTests()
    {
        gameId = Guid.NewGuid();
        playerId = Guid.NewGuid();
        gameName = "Test Game";
        recipes = new List<Recipe>
        {
            new Recipe(Guid.NewGuid(), "Chocolate Cake", new List<RecipeIngredient>()),
            new Recipe(Guid.NewGuid(), "Apple Pie", new List<RecipeIngredient>())
        };
    }

    [Fact]
    public async Task CreateNewGameAsync_ShouldCreateGameAndInitializeState()
    {
        // Arrange
        guidProviderMock.Setup(g => g.NewGuid())
            .Returns(gameId);

        recipeRepositoryMock
            .Setup(r => r.GetAllRecipesAsync())
            .ReturnsAsync(recipes);

        // Add all mock objects to the Silo
        Silo.AddService(guidProviderMock.Object);
        Silo.AddService(recipeRepositoryMock.Object);

        // Add probe to verify interactions with the game grain
        var gameGrainMock = Silo.AddProbe<IGameGrain>(gameId);

        // Act
        var grain = await Silo.CreateGrainAsync<GameFactoryGrain>(Guid.Empty);
        var actualGameId = await grain.CreateNewGameAsync(playerId, gameName);

        // Assert
        Assert.Equal(gameId, actualGameId);
        gameGrainMock.Verify(
            g => g.InitializeGameStateAsync(It.Is<InitializeGameStateCommand>(cmd =>
                cmd.GameId == actualGameId &&
                cmd.PlayerId == playerId &&
                cmd.GameName == gameName &&
                cmd.RecipeIds.SequenceEqual(recipes.Select(r => r.Id))
            )),
            Times.Once
        );
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
