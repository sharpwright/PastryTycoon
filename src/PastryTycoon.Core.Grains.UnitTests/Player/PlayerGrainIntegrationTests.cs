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
    public async Task DiscoverRecipeAsync_Should_Discover_New_Recipe()
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
    public async Task DiscoverRecipeAsync_Should_Not_Discover_Recipe_More_Than_Once()
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

    // [Fact]
    // public async Task UnlockAchievementAsync_Should_Unlock_New_Achievement()
    // {
    //     // Arrange
    //     var playerId = Guid.NewGuid();
    //     var achievementId = "FirstBake";
    //     var unlockedAtUtc = DateTime.UtcNow;
    //     var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

    //     // Act
    //     await grain.UnlockAchievementAsync(achievementId, unlockedAtUtc);

    //     // Assert
    //     Assert.True(received, "No events received within timeout.");
    //     Assert.Single(events, evt =>
    //         evt is AchievementUnlockedEvent e &&
    //         e.AchievementId == achievementId &&
    //         e.UnlockedAtUtc == unlockedAtUtc);
    // }
}
