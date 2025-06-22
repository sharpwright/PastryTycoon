using System;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;
using PastryTycoon.Core.Grains.Player;
using Xunit.Abstractions;

namespace PastryTycoon.Core.Grains.IntegrationTests.Player;

/// <summary>
/// Integration tests for the PlayerGrain.
/// </summary>
[Collection(ClusterCollection.Name)]
public class PlayerGrainIntegrationTests(ClusterFixture fixture)
{
    private readonly TestCluster cluster = fixture.Cluster;

    [Fact]
    public async Task TryDiscoverRecipeAsync_ShouldEmitPlayerDiscoveredRecipeEvent_WhenRecipeNotAlreadyDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var ingredientIds = new List<string> { "ingredient-1", "ingredient-2" };
        var command = new TryDiscoverRecipeCmd(playerId, ingredientIds);

        // Setup the grain and observers before testing
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act 1 - Initialize the player
        var initCommand = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var initResult = await grain.InitializeAsync(initCommand);
        Assert.True(initResult.IsSuccess);

        // Act 2 - Try to discover a recipe
        var discoveryResult = await grain.TryDiscoverRecipeFromIngredientsAsync(command);        
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();
        Assert.True(discoveryResult.IsSuccess);
        Assert.True(received, "No events received within timeout.");
        Assert.Single(events, evt =>
            evt is PlayerDiscoveredRecipeEvent e &&
            e.RecipeId == "test-recipe-id");        
    }

    [Fact]
    public async Task TryDiscoverRecipeAsync_ShouldNotEmitPlayerDiscoveredRecipeEvent_WhenRecipeAlreadyDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var ingredientIds = new List<string> { "ingredient-1", "ingredient-2" };
        var discoverCommand = new TryDiscoverRecipeCmd(playerId, ingredientIds);

        // Initialize the grain state before testing
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act 1 - Initialize the player
        var initCommand = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var initResult = await grain.InitializeAsync(initCommand);
        Assert.True(initResult.IsSuccess);

        // Act 2 - Try to discover a recipe
        var discoveryResult1 = await grain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();
        Assert.True(discoveryResult1.IsSuccess);
        Assert.True(received, "No events received within timeout.");
        Assert.Single(events, evt =>
            evt is PlayerDiscoveredRecipeEvent e &&
            e.RecipeId == "test-recipe-id");

        // Clear events for the next test
        await observer.ClearReceivedEventsAsync();

        // Act 3 - Try to discover the same recipe again
        var discoveryResult2 = await grain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);
        received = await observer.WaitForReceivedEventsAsync();
        events = await observer.GetReceivedEventsAsync();
        Assert.True(discoveryResult2.IsSuccess);                        // Check if the command was successful
        Assert.False(received, "Events received within timeout.");      // No new events should be emitted
        Assert.Empty(events);                                           // Ensure no new events were emitted
    }

    [Fact]
    public async Task InitializeAsync_ShouldInitializePlayer()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var command = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act
        await grain.InitializeAsync(command);
        var playerStatistics = await grain.GetPlayerStatisticsAsync();

        // Assert
        Assert.NotNull(playerStatistics);
        Assert.Equal(command.PlayerName, playerStatistics.PlayerName);
        Assert.Equal(playerId, playerStatistics.PlayerId);
        Assert.Equal(0, playerStatistics.TotalAchievementsUnlocked);
        Assert.Equal(0, playerStatistics.TotalRecipesCrafted);
        Assert.Equal(0, playerStatistics.TotalRecipesDiscovered);
        Assert.True(playerStatistics.CreatedAtUtc <= DateTime.UtcNow, "CreatedAtUtc should be less than or equal to current time.");
    }

    [Fact]
    public async Task InitializeAsync_ShouldReturnFailure_WhenPlayerAlreadyInitialized()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var command = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act 1
        var result = await grain.InitializeAsync(command);
        Assert.True(result.IsSuccess);

        // Act 2 - Attempt to re-initialize the player
        var secondResult = await grain.InitializeAsync(command);
        Assert.False(secondResult.IsSuccess);
    }

    [Fact]
    public async Task UnlockAchievementAsync_ShouldUnlockAchievement()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockedAtUtc = DateTime.UtcNow;
        var initPlayerCmd = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var updateCommand = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act 1
        var initResult = await grain.InitializeAsync(initPlayerCmd);
        Assert.True(initResult.IsSuccess);

        // Act 2 - Unlock the achievement
        var unlockResult = await grain.UnlockAchievementAsync(updateCommand);
        Assert.True(unlockResult.IsSuccess);

        // Act 3 - Retrieve player statistics
        var playerStatistics = await grain.GetPlayerStatisticsAsync();
        Assert.NotNull(playerStatistics);
        Assert.Equal(1, playerStatistics.TotalAchievementsUnlocked);
    }

    [Fact]
    public async Task UnlockAchievementAsync_ShouldReturnFailure_WhenAchievementAlreadyUnlocked()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockedAtUtc = DateTime.UtcNow;
        var initPlayerCmd = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        var command = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act 1
        var initResult = await grain.InitializeAsync(initPlayerCmd);
        Assert.True(initResult.IsSuccess);

        // Act 2 - Unlock the achievement for the first time
        var unlockResult1 = await grain.UnlockAchievementAsync(command);
        Assert.True(unlockResult1.IsSuccess);

        // Act 3 - Attempt to unlock the same achievement again
        var unlockResults2 = await grain.UnlockAchievementAsync(command);
        Assert.False(unlockResults2.IsSuccess);

        // Act 4 - Retrieve player statistics (should still be 1)
        var playerStatistics = await grain.GetPlayerStatisticsAsync();
        Assert.NotNull(playerStatistics);
        Assert.Equal(1, playerStatistics.TotalAchievementsUnlocked);
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_ShouldReturnInitializedStatistics()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var command = new InitPlayerCmd("TestPlayer", Guid.NewGuid());
        await grain.InitializeAsync(command);

        // Act
        var playerStatistics = await grain.GetPlayerStatisticsAsync();

        // Assert
        Assert.NotNull(playerStatistics);
        Assert.Equal(command.PlayerName, playerStatistics.PlayerName);
        Assert.Equal(playerId, playerStatistics.PlayerId);
        Assert.Equal(0, playerStatistics.TotalAchievementsUnlocked);
        Assert.Equal(0, playerStatistics.TotalRecipesCrafted);
        Assert.Equal(0, playerStatistics.TotalRecipesDiscovered);
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_ShouldThrow_WhenPlayerNotInitialized()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act & Assert: Verify getting statistics throws an exception
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => grain.GetPlayerStatisticsAsync());
        Assert.Equal("Player is not initialized.", exception.Message);
    }
}
