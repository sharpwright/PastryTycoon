using Xunit;
using PastryTycoon.Grains.Validation;
using PastryTycoon.Common.Commands;
using PastryTycoon.Grains.States;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace PastryTycoon.Grains.UnitTests.Validation
{
    public class InitializeGameStateCommandValidatorTestsTest
    {
        [Fact]
        public async Task Validate_ShouldReturnTrue_ForValidCommand()
        {
            // Arrange            
            var primaryKey = Guid.NewGuid();
            var gameId = primaryKey;
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, "Test Game", DateTime.UtcNow);
            var validator = new InitializeGameStateCommandValidator();

            var state = new GameState
            {
                GameId = gameId,
                PlayerId = playerId,
                DiscoverableRecipeIds = recipeIds.ToImmutableList(),
                GameName = "Test Game",
                StartTimeUtc = DateTime.UtcNow
            };

            // Act
            var result = await validator.ValidateCommandAsync(command, state, primaryKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_Throws_Argument_Exception_When_GameId_Doesnt_Match_PrimaryKey()
        {
            // Arrange            
            var primaryKey = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();
            var recipeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new InitializeGameStateCommand(gameId, playerId, recipeIds, "Test Game", DateTime.UtcNow);
            var validator = new InitializeGameStateCommandValidator();

            var state = new GameState
            {
                GameId = gameId,
                PlayerId = playerId,
                DiscoverableRecipeIds = recipeIds.ToImmutableList(),
                GameName = "Test Game",
                StartTimeUtc = DateTime.UtcNow
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                validator.ValidateCommandAsync(command, state, primaryKey));

            Assert.Contains("GameId must match grain primary key", exception.Message);
        }

    }
}