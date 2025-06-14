using Xunit;
using System.Collections.Immutable;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Grains.Game;
using Orleans.Providers.Streams.Generator;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators
{
    public class InitGameStateCmdValTests
    {
        [Fact]
        public async Task Validate_ShouldReturnValidResult_WhenCommandIsValid()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.NewGuid(),
                ["test-recipe-1", "test-recipe-2"],
                DateTime.UtcNow);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenGameIdIsEmpty()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.Empty,
                Guid.NewGuid(),
                ["test-recipe-1", "test-recipe-2"],
                DateTime.UtcNow);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.GameId));
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenPlayerIdIsEmpty()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.Empty,
                ["test-recipe-1", "test-recipe-2"],
                DateTime.UtcNow);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.PlayerId));
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenRecipesIdsIsEmpty()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.NewGuid(),
                [],
                DateTime.UtcNow);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.RecipeIds));
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenRecipesIdsIsNull()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null!,
                DateTime.UtcNow);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.RecipeIds));
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenStartTimeUtcIsEmpty()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.NewGuid(),
                ["test-recipe-1", "test-recipe-2"],
                default);

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.StartTimeUtc));
        }

        [Fact]
        public async Task Validate_ShouldReturnInvalidResult_WhenStartTimeIsInTheFuture()
        {
            // Arrange
            var command = new InitGameStateCmd(
                Guid.NewGuid(),
                Guid.NewGuid(),
                ["test-recipe-1", "test-recipe-2"],
                DateTime.UtcNow.AddHours(1));

            var validator = new InitGameStateCmdVal();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitGameStateCmd.StartTimeUtc));
        }
    } 
}