using System;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;

namespace PastryTycoon.Core.Grains.UnitTests.Player;

[Collection(ClusterCollection.Name)]
public class PlayerGrainIntegrationTests(ClusterFixture fixture)
{
    private readonly TestCluster cluster = fixture.Cluster;

    [Fact]
    public async Task DiscoverRecipeAsync_ShouldDiscoverNewRecipe()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow);
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act
        await grain.DiscoverRecipeAsync(command);
        var received = await observer.WaitForReceivedEventsAsync();
        var events = await observer.GetReceivedEventsAsync();

        // Assert
        Assert.True(received, "No events received within timeout.");
        Assert.Single(events, evt =>
            evt is PlayerDiscoveredRecipeEvent e &&
            e.RecipeId == recipeId);
    }

    [Fact]
    public async Task DiscoverRecipeAsync_ShouldThrow_WhenRecipeAlreadyDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<PlayerEvent>>(playerId);
        await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

        // Act
        var command = new DiscoverRecipeCommand(playerId, Guid.NewGuid(), DateTime.UtcNow);
        await grain.DiscoverRecipeAsync(command);

        // Assert: Verify second call throws an exception
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => grain.DiscoverRecipeAsync(command));
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
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => grain.InitializeAsync(command));
        Assert.Equal("Player is already initialized.", exception.Message);
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
