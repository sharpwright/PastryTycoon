using System;
using System.Threading.Tasks;
using PastryTycoon.Common.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Streams;
using Xunit;
using Orleans.TestKit;
using PastryTycoon.Common.Constants;
using System.Reflection;
using Orleans.Streams.Core;
using PastryTycoon.Common.EventHandlers;
using PastryTycoon.Grains.EventHandlers;

namespace PastryTycoon.Grains.UnitTests.EventHandlers
{
    public class GameProjectionGrainTests : TestKitBase
    {
        private readonly Mock<ILogger<IGameProjectionGrain>> loggerMock;
        private readonly GameProjectionGrain grain;

        public GameProjectionGrainTests()
        {
            loggerMock = new Mock<ILogger<IGameProjectionGrain>>();
            grain = new GameProjectionGrain(loggerMock.Object);
        }

        [Fact]
        public void ImplicitGameProjectionGrain_Has_Required_Attribute()
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
        public void ImplicitGameProjectionGrain_Implements_Required_Interfaces()
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
        public async Task Handle_GameStartedEvent_LogsInformation()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };            
            var evt = new GameStateInitializedEvent(gameId, playerId, recipeIds, "TestGame", DateTime.UtcNow);

            // Act
            await grain.Handle(evt);

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
        public async Task Handle_GameUpdatedEvent_LogsInformation()
        {
            // Arrange
            var evt = new GameUpdatedEvent(Guid.NewGuid(), "TestGame", DateTime.UtcNow);

            // Act
            await grain.Handle(evt);

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
        public async Task OnErrorAsync_LogsError()
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