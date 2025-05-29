using System;
using System.Linq.Expressions;
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

    [Theory]
    [MemberData(nameof(AchievementsTestData.UnlockAchievementIdForGivenEventAndState), MemberType = typeof(AchievementsTestData))]
    public async Task OnNextAsync_ShouldUpdateState_AndShouldUnlockAchievementIdForGivenEventAndState(
        PlayerEvent playerEvent,
        AchievementsState grainState,
        string expectedAchievementId,
        Expression<Func<AchievementsState, bool>> stateValidationExpression)
    {
        // Arrange
        var player = new Mock<IPlayerGrain>();
        Silo.AddProbe(identity => player);
        Silo.AddService(loggerMock.Object);
        Silo.AddPersistentState(OrleansConstants.GRAIN_STATE_ACHIEVEMENTS, OrleansConstants.GRAIN_STATE_ACHIEVEMENTS, grainState);
        var grain = await Silo.CreateGrainAsync<AchievementsGrain>(playerEvent.PlayerId);

        // Act
        await grain.OnNextAsync(playerEvent);

        // Assert  
        Assert.True(stateValidationExpression.Compile().Invoke(grainState),
            $"""
            State mismatch detected!
            Current State: {grainState}
            Expected New State Condition: {stateValidationExpression.Body}
            Check if the event updates the state correctly.
            """);

        // Verify behaviour
        player.Verify(
            x => x.UnlockAchievementAsync(expectedAchievementId, It.IsAny<DateTime>()),
            Times.Once,
            $"""
            Achievement unlock verification failed!
            Expected player grain to be called to unlock the achievement '{expectedAchievementId}'.
            """);
    }
}
