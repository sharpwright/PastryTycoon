using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Streams;
using Orleans.Streams.Core;
using Orleans.TestKit;
using PastryTycoon.Core.Abstractions.Achievements;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Achievements;
using PastryTycoon.Core.Grains.Achievements.UnlockHandlers;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.UnitTests.Achievements;

public class AchievementsGrainTests : TestKitBase
{
    private readonly Mock<ILogger<IAchievementsGrain>> loggerMock;

    public AchievementsGrainTests() 
    {
        loggerMock = new Mock<ILogger<IAchievementsGrain>>();          
    }

    [Fact]
    public void AchievementsGrain_Has_Required_Attribute()
    {
        // Arrange
        var grainType = typeof(AchievementsGrain);

        //  Act
        var attribute = grainType.GetCustomAttribute<ImplicitStreamSubscriptionAttribute>();

        // Assert
        Assert.NotNull(attribute);
        Assert.True(attribute.Predicate.IsMatch(OrleansConstants.STREAM_NAMESPACE_PLAYER_EVENTS));
    }


    [Fact]
    public void ImplicitGameProjectionGrain_Implements_Required_Interfaces()
    {
        // Arrange
        var grainType = typeof(AchievementsGrain);

        // Assert
        Assert.True(typeof(IAsyncObserver<PlayerEvent>).IsAssignableFrom(grainType),
            "Should implement IAsyncObserver<PlayerEvent>");
        Assert.True(typeof(IStreamSubscriptionObserver).IsAssignableFrom(grainType),
            "Should implement IStreamSubscriptionObserver");
    }

    [Fact]
    public async Task Handle_RecipeDiscoveredEvent_UnlocksAchievement()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var evt = new RecipeDiscoveredEvent(playerId, recipeId, DateTime.UtcNow);
        var state = new AchievementsState
        {
            RecipesDiscovered = 0,
            RareIngredientsUsed = new()
        };

        Silo.AddService(loggerMock.Object);
        Silo.AddPersistentState(OrleansConstants.GRAINS_STATE_ACHIEVEMENTS, OrleansConstants.GRAINS_STATE_ACHIEVEMENTS, state);

        var player = new Mock<IPlayerGrain>();
        Silo.AddProbe(identity => player);

        var grain = await Silo.CreateGrainAsync<AchievementsGrain>(playerId);

        // Act
        await grain.OnNextAsync(evt);

        // Assert            
        Assert.Equal(1, state.RecipesDiscovered);

        // Verify behaviour
        player.Verify(
            x => x.UnlockAchievementAsync(AchievementConstants.FIRST_RECIPE_DISCOVERED, It.IsAny<DateTime>()),
            Times.Once,
            "Player grain should be called to unlock the achievement.");
    }
}
