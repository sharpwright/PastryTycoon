using BakerySim.Common.Orleans;
using Orleans.TestKit;
using Xunit;
using Moq;
using BakerySim.Grains.Actors;
using BakerySim.Grains.Commands;
using BakerySim.Grains.Events;
using Orleans.TestingHost;
using BakerySim.Grains.UnitTests.TestClusterHelpers;

namespace BakerySim.Grains.UnitTests.Actors
{
    /// <summary>
    /// Tests for the GameGrain actor using Orleans Testinghost.
    /// </summary>
    /// <remarks>
    /// The OrleansTestkit doesn't support event sourcing with JournaledGrains.
    /// Therefore, we use the Orleans Testinghost to test the GameGrain actor.
    /// Silo configurations are defined in the ClusterFixture class.
    /// </remarks>
    /// <param name="fixture"></param>
    [Collection(ClusterCollection.Name)]
    public class GameGrainTests(ClusterFixture fixture)// : TestKitBase
    {
        private readonly TestCluster cluster = fixture.Cluster;

        [Fact]
        public async Task StartGame_Should_Send_GameStartedEvent()
        {
            // Arrange           
            var gameId = Guid.NewGuid();
            var command = new StartGameCommand(gameId, "Test Game", DateTime.UtcNow);
            var grain = this.cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
            var observer = this.cluster.GrainFactory.GetGrain<IStreamObserverGrain<GameEvent>>(gameId);
            await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

            // Act
            await grain.StartGame(command);
            var received = await observer.WaitForReceivedEventsAsync();
            var events = await observer.GetReceivedEventsAsync();

            // Assert
            Assert.True(received, "No events received within timeout.");
            Assert.Single(events, evt =>
                evt is GameStartedEvent &&
                evt.GameId == gameId &&
                evt.GameName == "Test Game");
        }
        
        /// <summary>
        /// Example of a unit test using the Orleans Teskit.
        /// </summary>
        /// <returns></returns>
        // [Fact]
        // public async Task StartGame_Should_Send_GameStartedEvent()
        // {
        //     // Arrange
        //     var gameId = Guid.NewGuid();
        //     var command = new StartGameCommand(gameId, "Test Game", DateTime.UtcNow);
        //     var stream = Silo.AddStreamProbe<GameStartedEvent>(
        //         gameId, OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        //     var grain = await Silo.CreateGrainAsync<GameGrain>(gameId);

        //     // Act
        //     await grain.StartGame(command);

        //     // Assert
        //     stream.VerifySend(evt => evt.GameId == gameId, Times.Once());
        // }
    }
}