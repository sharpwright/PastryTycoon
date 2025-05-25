using System;
using Orleans.TestingHost;
using PastryTycoon.Common.Actors;
using PastryTycoon.Common.Commands;
using PastryTycoon.Grains.UnitTests.TestClusterHelpers;

namespace PastryTycoon.Grains.UnitTests.Actors;

/// <summary>
///     Contains unit tests to verify correct behavior for game state initialization,
///     event emission, validation, and state persistence across multiple commands.
/// </summary>
/// <remarks>
///     <para>
///         These tests follow best practices for testing Orleans grains using CQRS and event sourcing patterns:
///         <list type="bullet">
///             <item>Interact with the grain exclusively through its public interface (commands and queries).</item>
///             <item>Issue sequences of commands (e.g., <c>InitializeGameStateAsync</c>, <c>UpdateGameAsync</c>).</item>
///             <item>Verify observable state via query/read model methods (e.g., <c>GetGameStatisticsAsync</c>), not by accessing internal state directly.</item>
///             <item>Ensure that event replay and state transitions function as intended by testing the effects of command sequences.</item>
///         </list>
///     </para>
///     <para>
///         The tests include:
///         <list type="number">
///             <item>Verifying that initializing game state sets the correct values.</item>
///             <item>Ensuring a <c>GameStateInitializedEvent</c> is emitted upon initialization.</item>
///             <item>Validating that an exception is thrown if the command's GameId does not match the grain's ID.</item>
///             <item>Testing that the grain persists and updates state correctly across multiple commands.</item>
///         </list>
///     </para>
/// </remarks>
/// <returns></returns>
[Collection(ClusterCollection.Name)]
public class GameGrainSequenceTests(ClusterFixture fixture)
{
    private readonly TestCluster cluster = fixture.Cluster;

    [Fact]
    public async Task Sequence_Of_Commands_Should_Persist_State_Correctly()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var initializeGameStateCommand = new InitializeGameStateCommand(gameId, playerId, recipeIds, "Test Game", DateTime.UtcNow);
        var updateGameCommand = new UpdateGameCommand(gameId, "Updated Game", DateTime.UtcNow.AddMinutes(10));
        var gameGrain = this.cluster.GrainFactory.GetGrain<IGameGrain>(gameId);

        // Act
        await gameGrain.InitializeGameStateAsync(initializeGameStateCommand);
        await gameGrain.UpdateGameAsync(updateGameCommand);
        var gameStatistics = await gameGrain.GetGameStatisticsAsync(gameId);

        // Assert
        Assert.Equal(gameId, gameStatistics.GameId);
        Assert.Equal(playerId, gameStatistics.PlayerId);
        Assert.Equal("Updated Game", gameStatistics.GameName);
    }
}
