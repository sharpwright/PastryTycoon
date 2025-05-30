using PastryTycoon.Core.Abstractions.Constants;
using Orleans.TestingHost;
using PastryTycoon.Core.Grains.UnitTests.TestClusterHelpers;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Abstractions.Game;

namespace PastryTycoon.Core.Grains.UnitTests.Game
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
        public async Task InitializeGameState_Should_Set_GameState()
        {
            // Arrange           
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var startTimeUtc = DateTime.UtcNow;
            var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, startTimeUtc);
            var grain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

            // Act
            await grain.InitializeGameStateAsync(command);
            var gameStatistics = await grain.GetGameStatisticsAsync(gameId);

            // Assert
            Assert.Equal(gameId, gameStatistics.GameId);
            Assert.Equal(playerId, gameStatistics.PlayerId);
            Assert.Equal(recipeIds.Count, gameStatistics.TotalRecipes);
            Assert.Equal(startTimeUtc, gameStatistics.StartTimeUtc);
        }

        [Fact]
        public async Task InitializeGameState_Should_Send_GameInitializedEvent()
        {
            // Arrange           
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, DateTime.UtcNow);
            var grain = cluster.GrainFactory.GetGrain<IGameGrain>(gameId);
            var observer = cluster.GrainFactory.GetGrain<IStreamObserverGrain<GameEvent>>(gameId);
            await observer.SubscribeAsync(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);

            // Act
            await grain.InitializeGameStateAsync(command);
            var received = await observer.WaitForReceivedEventsAsync();
            var events = await observer.GetReceivedEventsAsync();

            // Assert
            Assert.True(received, "No events received within timeout.");
            Assert.Single(events, evt =>
                evt is GameStateInitializedEvent e &&
                e.GameId == gameId);
        }     
    }
}