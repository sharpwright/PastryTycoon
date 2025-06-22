using System;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Common;
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
    public async Task InitializeAsync_ShouldInitializePlayer()
    {
        // Arrange
        var initCmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new InitPlayerCmd(initCmdId, "TestPlayer", Guid.NewGuid());
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
        var initCmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new InitPlayerCmd(initCmdId, "TestPlayer", Guid.NewGuid());
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
        var initCmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockCmdId = Guid.NewGuid();
        var unlockedAtUtc = DateTime.UtcNow;
        var initPlayerCmd = new InitPlayerCmd(initCmdId, "TestPlayer", Guid.NewGuid());
        var updateCommand = new UnlockAchievementCmd(unlockCmdId, playerId, achievementId, unlockedAtUtc);
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
        var initCmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var achievementId = "FirstBake";
        var unlockCmdId = Guid.NewGuid();
        var unlockedAtUtc = DateTime.UtcNow;
        var initPlayerCmd = new InitPlayerCmd(initCmdId,"TestPlayer", Guid.NewGuid());
        var command = new UnlockAchievementCmd(unlockCmdId, playerId, achievementId, unlockedAtUtc);
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
        var initCmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var grain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var command = new InitPlayerCmd(initCmdId, "TestPlayer", Guid.NewGuid());
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
