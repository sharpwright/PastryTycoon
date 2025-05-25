using PastryTycoon.Common.Constants;
using Orleans.TestKit;
using Xunit;
using Moq;
using PastryTycoon.Common.Actors;
using PastryTycoon.Common.Commands;
using PastryTycoon.Common.Events;
using Orleans.TestingHost;
using PastryTycoon.Grains.UnitTests.TestClusterHelpers;

namespace PastryTycoon.Grains.UnitTests.Actors
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
    public class GameGrainTests(ClusterFixture fixture)
    {
        private readonly TestCluster cluster = fixture.Cluster;

        [Fact]
        public async Task StartGame_Should_Send_GameStartedEvent()
        {
            // Arrange           
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, "Test Game", DateTime.UtcNow);
            var grain = this.cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
            var observer = this.cluster.GrainFactory.GetGrain<IStreamObserverGrain<GameEvent>>(gameId);
            await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

            // Act
            await grain.InitializeGameState(command);
            var received = await observer.WaitForReceivedEventsAsync();
            var events = await observer.GetReceivedEventsAsync();

            // Assert
            Assert.True(received, "No events received within timeout.");
            Assert.Single(events, evt =>
                evt is GameStateInitializedEvent e &&
                e.GameId == gameId &&
                e.GameName == "Test Game");
        }                
    }
}