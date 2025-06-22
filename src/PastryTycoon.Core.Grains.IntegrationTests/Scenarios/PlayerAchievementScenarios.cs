using System;
using System.Diagnostics;
using Orleans.TestingHost;
using PastryTycoon.Core.Abstractions.Achievements;
using PastryTycoon.Core.Abstractions.Common;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.IntegrationTests.TestClusterHelpers;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.IntegrationTests.Scenarios;

[Collection(ClusterCollection.Name)]
public class PlayerAchievementScenarios(ClusterFixture fixture)
{
    private readonly TestCluster cluster = fixture.Cluster;

    [Fact]
    public async Task DiscoveringFirstRecipe_ShouldEventuallyUnlockAchievement()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerGrain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);

        // Act 1: Initialize the player. This is part of the setup.
        var initCommand = new InitPlayerCmd(Guid.NewGuid(), "TestPlayer", Guid.NewGuid());
        await playerGrain.InitializeAsync(initCommand);

        // Act 2: Discover a recipe. This is the action that triggers the entire reactive flow.
        // PlayerGrain emits event -> AchievementsGrain receives event -> AchievementsGrain sends command -> PlayerGrain receives command
        var discoverCommand = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, new List<string> { "ingredient-1", "ingredient-2" });
        await playerGrain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);

        // Assert: Poll for the final state.
        await PollUntil(async () =>
        {
            var stats = await playerGrain.GetPlayerStatisticsAsync();
            return stats.TotalAchievementsUnlocked == 1;
        });

        // Final verification
        var finalStats = await playerGrain.GetPlayerStatisticsAsync();
        Assert.Equal(1, finalStats.TotalAchievementsUnlocked);
    }

    [Fact]
    public async Task DiscoveringSameRecipeTwice_ShouldNotChangePlayerState()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerGrain = cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var discoverCommand = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, new List<string> { "ingredient-1", "ingredient-2" });

        // Act 1: Initialize and discover the recipe for the first time.
        await playerGrain.InitializeAsync(new InitPlayerCmd(Guid.NewGuid(), "TestPlayer", Guid.NewGuid()));
        await playerGrain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);

        // Assert 1: Wait for the initial state update (discovery + achievement).
        await PollUntil(async () =>
        {
            var stats = await playerGrain.GetPlayerStatisticsAsync();
            return stats.TotalRecipesDiscovered == 1 && stats.TotalAchievementsUnlocked == 1;
        });

        // Act 2: Discover the exact same recipe again.
        await playerGrain.TryDiscoverRecipeFromIngredientsAsync(discoverCommand);

        // Give the system a moment to process, just in case any incorrect events were fired.
        await Task.Delay(1000);

        // Assert 2: Verify that the state has not changed.
        var finalStats = await playerGrain.GetPlayerStatisticsAsync();
        Assert.NotNull(finalStats);
        Assert.Equal(1, finalStats.TotalRecipesDiscovered);
        Assert.Equal(1, finalStats.TotalAchievementsUnlocked);
    }
    
    /// <summary>
    /// Polls an asynchronous condition until it returns true or a timeout is reached.
    /// </summary>
    /// <param name="condition">The async function that returns true when the condition is met.</param>
    /// <param name="timeoutMilliseconds">The total time to wait.</param>
    /// <param name="pollIntervalMilliseconds">The time to wait between checks.</param>
    /// <exception cref="TimeoutException">Thrown if the condition is not met within the timeout.</exception>
    private static async Task PollUntil(Func<Task<bool>> condition, int timeoutMilliseconds = 5000, int pollIntervalMilliseconds = 500)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds)
        {
            if (await condition())
            {
                return; // Condition met, success.
            }
            await Task.Delay(pollIntervalMilliseconds);
        }

        throw new TimeoutException("The polling condition was not met within the specified timeout.");
    }
}
