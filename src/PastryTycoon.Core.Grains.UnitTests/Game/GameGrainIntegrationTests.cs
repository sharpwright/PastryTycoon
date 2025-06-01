using System;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

namespace PastryTycoon.Core.Grains.UnitTests.Game;

/// <summary>
///     Contains unit tests to verify correct behavior for game state initialization,
///     event emission, validation, and state persistence across multiple commands.
/// </summary>
/// <remarks>
///     <para>
///         These tests follow best practices for testing Orleans grains using CQRS and event sourcing patterns:
///         <list type="bullet">
///             <item>Interact with the grain exclusively through its public interface (commands and queries).</item>
///             <item>Issue sequences of commands (e.g., <c>InitializeGameStateAsync</c>, <c>UpdateGameAsync</c>).</item>
///             <item>Verify observable state via query/read model methods (e.g., <c>GetGameStatisticsAsync</c>), not by accessing internal state directly.</item>
///             <item>Ensure that event replay and state transitions function as intended by testing the effects of command sequences.</item>
///         </list>
///     </para>
///     <para>
///         The tests include:
///         <list type="number">
///             <item>Verifying that initializing game state sets the correct values.</item>
///             <item>Ensuring a <c>GameStateInitializedEvent</c> is emitted upon initialization.</item>
///             <item>Validating that an exception is thrown if the command's GameId does not match the grain's ID.</item>
///             <item>Testing that the grain persists and updates state correctly across multiple commands.</item>
///         </list>
///     </para>
/// </remarks>
/// <returns></returns>
[Collection(ClusterCollection.Name)]
public class GameGrainIntegrationTests(ClusterFixture fixture)
{
    private readonly TestCluster cluster = fixture.Cluster;

    [Fact]
    public async Task InitializeGameState_ShouldSetInitialValues()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var initializeGameStateCommand = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
        var gameGrain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act
        await gameGrain.InitializeGameStateAsync(initializeGameStateCommand);
        var gameStatistics = await gameGrain.GetGameStatisticsAsync();

        // Assert
        Assert.Equal(gameId, gameStatistics.GameId);
        Assert.Equal(playerId, gameStatistics.PlayerId);
        Assert.Equal(recipeIds.Count, gameStatistics.TotalRecipes);
    }

    [Fact]
    public async Task InitializeGameState_ShouldThrowException_WhenGameIdDoesNotMatchGrainId()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var initializeGameStateCommand = new InitializeGameStateCommand(Guid.NewGuid(), playerId, recipeIds, DateTime.UtcNow);
        var gameGrain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => gameGrain.InitializeGameStateAsync(initializeGameStateCommand));
    }

    [Fact]
    public async Task InitializeGameState_ShouldEmitGameStateInitializedEvent()
    {
        // Arrange           
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
        var grain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<GameEvent>>(gameId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act
        await grain.InitializeGameStateAsync(command);
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();

        // Assert
        Assert.True(received, "No events received within timeout.");
        Assert.Single(events, evt =>
            evt is GameStateInitializedEvent e &&
            e.GameId == gameId);
    }

    [Fact]
    public async Task UpdateGame_ShouldPersistUpdatedValues()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var initializeGameStateCommand = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
        var updateGameCommand = new UpdateGameCommand(gameId, DateTime.UtcNow.AddMilliseconds(1));
        var gameGrain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act
        await gameGrain.InitializeGameStateAsync(initializeGameStateCommand);
        var gameStatistics = await gameGrain.GetGameStatisticsAsync();
        var lastUpdated = gameStatistics.LastUpdatedUtc;

        await gameGrain.UpdateGameAsync(updateGameCommand);
        gameStatistics = await gameGrain.GetGameStatisticsAsync();

        // Assert
        Assert.Equal(gameId, gameStatistics.GameId);
        Assert.Equal(playerId, gameStatistics.PlayerId);
        Assert.True(lastUpdated < gameStatistics.LastUpdatedUtc,
            "Last updated time should be updated after the UpdateGame command.");
    }

    [Fact]
    public async Task GetGameStatisticsAsync_ShouldReturnInitializedStatistics()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var initializeGameStateCommand = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
        var gameGrain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act
        await gameGrain.InitializeGameStateAsync(initializeGameStateCommand);
        var gameStatistics = await gameGrain.GetGameStatisticsAsync();

        // Assert
        Assert.NotNull(gameStatistics);
        Assert.Equal(gameId, gameStatistics.GameId);
        Assert.Equal(playerId, gameStatistics.PlayerId);
        Assert.Equal(recipeIds.Count, gameStatistics.TotalRecipes);
    }

    [Fact]
    public async Task GetGameStatisticsAsync_ShouldThrow_WhenPlayerNotInitialized()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameGrain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => gameGrain.GetGameStatisticsAsync());
        Assert.Equal("Game state is not initialized. Please initialize the game first.", exception.Message);
    }
}
