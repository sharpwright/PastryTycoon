using Xunit;
using System.Collections.Immutable;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Grains.Game;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators
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

            // Act & Assert 
            // Validation is successful when method completes without exception.
            var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAsync(command, state, primaryKey));
            Assert.Null(exception);
            
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