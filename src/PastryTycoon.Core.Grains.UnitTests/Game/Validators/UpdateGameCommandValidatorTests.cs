using System;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators;

public class UpdateGameCommandValidatorTests
{
    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ValidCommand_ShouldNotThrow()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameName = "Test Game";
        var updateTimeUtc = DateTime.UtcNow;
        var command = new UpdateGameCommand(gameId, gameName, updateTimeUtc);

        var validator = new UpdateGameCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, new(), gameId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldThrowArgumentException_WhenGameIdIsEmpty()
    {
        // Arrange
        var gameId = Guid.Empty; // Invalid game ID
        var gameName = "Test Game";
        var updateTimeUtc = DateTime.UtcNow;
        var command = new UpdateGameCommand(gameId, gameName, updateTimeUtc);

        var validator = new UpdateGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), gameId));

        Assert.Equal("'Command.GameId': 'GameId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldThrowArgumentException_WhenGameNameIsEmpty()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameName = string.Empty; // Invalid game name
        var updateTimeUtc = DateTime.UtcNow;
        var command = new UpdateGameCommand(gameId, gameName, updateTimeUtc);

        var validator = new UpdateGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), gameId));

        Assert.Equal("'Command.GameName': 'GameName is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldThrowArgumentException_WhenUpdateTimeUtcIsInFuture()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameName = "Test Game";
        var updateTimeUtc = DateTime.UtcNow.AddMinutes(10); // Invalid future time
        var command = new UpdateGameCommand(gameId, gameName, updateTimeUtc);

        var validator = new UpdateGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), gameId));

        Assert.Equal("'Command.UpdateTimeUtc': 'UpdateTimeUtc must be in the past or present' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldThrowArgumentException_WhenGameIdDoesNotMatchGrainPrimaryKey()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameName = "Test Game";
        var updateTimeUtc = DateTime.UtcNow;
        var command = new UpdateGameCommand(gameId, gameName, updateTimeUtc);
        var grainPrimaryKey = Guid.NewGuid(); // Different from gameId

        var validator = new UpdateGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), grainPrimaryKey));

        Assert.Equal("'Command.GameId': 'GameId must match grain primary key' (Parameter 'command')", exception.Message);
    }
}
