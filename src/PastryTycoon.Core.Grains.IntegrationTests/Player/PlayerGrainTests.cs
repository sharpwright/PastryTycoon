using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;
using PastryTycoon.Core.Grains.Player;
using Xunit.Abstractions;

namespace PastryTycoon.Core.Grains.UnitTests.Player;

/// <summary>
/// Integration tests for the PlayerGrain.
/// </summary>
[Collection(ClusterCollection.Name)]
public class PlayerGrainIntegrationTests(ClusterFixture fixture, ITestOutputHelper output)
{
    private readonly TestCluster cluster = fixture.Cluster;
    private readonly ITestOutputHelper output = output;

    [Fact]
    public async Task TryDiscoverRecipeAsync_ShouldEmitPlayerDiscoveredRecipeEvent_WhenRecipeNotDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var ingredientIds = new List<string> { "ingredient-1", "ingredient-2" };
        var command = new TryDiscoverRecipeCommand(playerId, ingredientIds);

        // Use a test helper to inject the mock into the grain if your test cluster supports DI overrides
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act
        await grain.TryDiscoverRecipeFromIngredientsAsync(command);
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();

        // Assert
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
        var discoverCommand = new TryDiscoverRecipeCommand(playerId, ingredientIds);

        // Initialize the grain state before testing
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Initialize the player
        var initializeCommand = new InitializePlayerCommand("TestPlayer", Guid.NewGuid());
        await grain.InitializeAsync(initializeCommand);

        // Simulate discovering a recipe
        await grain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();

        // Assert: Verify first discovery emits the event
        Assert.True(received, "No events received within timeout.");
        Assert.Single(events, evt =>
            evt is PlayerDiscoveredRecipeEvent e &&
            e.RecipeId == "test-recipe-id");

        // Clear events for the next test
        await observer.ClearReceivedEventsAsync();

        // Verify second discovery does not emit an event
        await grain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);
        received = await observer.WaitForReceivedEventsAsync();
        events = await observer.GetReceivedEventsAsync();

        // Assert: Verify second call does not emit any events
        Assert.False(received, "Events received within timeout.");
        Assert.Empty(events);
    }

    [Fact]
    public async Task InitializeAsync_ShouldInitializePlayer()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var command = new InitializePlayerCommand("TestPlayer", Guid.NewGuid());
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
    public async Task InitializeAsync_ShouldThrow_WhenPlayerAlreadyInitialized()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var command = new InitializePlayerCommand("TestPlayer", Guid.NewGuid());
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        await grain.InitializeAsync(command);

        // Act & Assert: Verify second initialization throws an exception
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => grain.InitializeAsync(command));
    }

    [Fact]
    public async Task UnlockAchievementAsync_ShouldUnlockAchievement()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockedAtUtc = DateTime.UtcNow;
        var initializeCommand = new InitializePlayerCommand("TestPlayer", Guid.NewGuid());
        var updateCommand = new UnlockAchievementCommand(playerId, achievementId, unlockedAtUtc);
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act
        await grain.InitializeAsync(initializeCommand);
        await grain.UnlockAchievementAsync(updateCommand);
        var playerStatistics = await grain.GetPlayerStatisticsAsync();

        // Assert
        Assert.NotNull(playerStatistics);
        Assert.Equal(1, playerStatistics.TotalAchievementsUnlocked);
    }

    [Fact]
    public async Task UnlockAchievementAsync_ShouldThrow_WhenAchievementAlreadyUnlocked()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCommand(playerId, achievementId, unlockedAtUtc);
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        await grain.UnlockAchievementAsync(command);

        // Act & Assert: Verify second unlock throws an exception
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => grain.UnlockAchievementAsync(command));
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_ShouldReturnInitializedStatistics()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var command = new InitializePlayerCommand("TestPlayer", Guid.NewGuid());
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
