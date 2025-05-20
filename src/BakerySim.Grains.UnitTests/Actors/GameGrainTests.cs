using BakerySim.Common.Orleans;
using Orleans.TestKit;
using Xunit;
using Moq;
using BakerySim.Grains.Actors;
using BakerySim.Grains.Commands;
using BakerySim.Grains.Events;

namespace BakerySim.Grains.UnitTests.Actors
{
    public class GameGrainTests : TestKitBase
    {
        public GameGrainTests()
        {
        }

        [Fact]
        public async Task StartGame_Should_Send_GameStartedEvent()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var command = new StartGameCommand(gameId, "Test Game", DateTime.UtcNow);
            var stream = Silo.AddStreamProbe<GameStartedEvent>(
                gameId, OrleansConstants.STREAM_GAME_NAMESPACE, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
            var grain = await Silo.CreateGrainAsync<GameGrain>(gameId);

            // Act
            await grain.StartGame(command);

            // Assert
            stream.VerifySend(evt => evt.GameId == gameId, Times.Once());
        }
    }
}