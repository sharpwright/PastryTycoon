using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Streams;
using PastryTycoon.Core.Abstractions.Constants;
using System.Reflection;
using Orleans.Streams.Core;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Abstractions.Game;

namespace PastryTycoon.Core.Grains.UnitTests.Game
{
    public class GameProjectionGrainTests
    {
        private readonly Mock<ILogger<IGameProjectionGrain>> loggerMock;
        private readonly GameProjectionGrain grain;

        public GameProjectionGrainTests()
        {
            loggerMock = new Mock<ILogger<IGameProjectionGrain>>();
            grain = new GameProjectionGrain(loggerMock.Object);
        }

        [Fact]
        public void ImplicitGameProjectionGrain_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var grainType = typeof(GameProjectionGrain);

            //  Act
            var attribute = grainType.GetCustomAttribute<ImplicitStreamSubscriptionAttribute>();

            // Assert
            Assert.NotNull(attribute);
            Assert.True(attribute.Predicate.IsMatch(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS));
        }


        [Fact]
        public void ImplicitGameProjectionGrain_ShouldImplementRequiredInterfaces()
        {
            // Arrange
            var grainType = typeof(GameProjectionGrain);

            // Assert
            Assert.True(typeof(IAsyncObserver<GameEvent>).IsAssignableFrom(grainType),
                "Should implement IAsyncObserver<GameEvent>");
            Assert.True(typeof(IStreamSubscriptionObserver).IsAssignableFrom(grainType),
                "Should implement IStreamSubscriptionObserver");
        }

        [Fact]
        public async Task HandleGameStartedEvent_ShouldLogInformation()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };            
            var evt = new GameStateInitializedEvent(gameId, playerId, recipeIds, DateTime.UtcNow);

            // Act
            await grain.HandleGameInitiliazedEventAsync(evt);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Game started at")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleGameUpdatedEvent_ShouldLogInformation()
        {
            // Arrange
            var evt = new GameUpdatedEvent(GameId: Guid.NewGuid(), UpdateTimeUtc: DateTime.UtcNow);

            // Act
            await grain.HandleGameUpdatedEventAsync(evt);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Game updated at")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task OnErrorAsync_ShouldLogError()
        {
            // Arrange
            var ex = new Exception("Test error");

            // Act
            await grain.OnErrorAsync(ex);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Error: Test error")),
                    ex,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}