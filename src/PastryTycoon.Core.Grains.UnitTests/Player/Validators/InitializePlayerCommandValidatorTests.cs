using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Player.Validators;

public class InitializePlayerCommandValidatorTests
{
    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldReturnTrue_WhenGrainStateIsNotInitialized()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var gameId = Guid.NewGuid();
        var command = new InitializePlayerCommand(playerName, gameId);
        var state = new PlayerState { IsInitialized = false };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, state, playerId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ShouldThrowInvalidOperationException_WhenGrainStateIsAlreadyInitialized()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var gameId = Guid.NewGuid();
        var command = new InitializePlayerCommand(playerName, gameId);
        var grainState = new PlayerState { IsInitialized = true };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'GrainState.IsInitialized': 'Player is already initialized' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldReturnTrue_WhenCommandIsValid()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var gameId = Guid.NewGuid();
        var command = new InitializePlayerCommand(playerName, gameId);
        var grainState = new PlayerState { IsInitialized = false };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenGameIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var gameId = Guid.Empty;
        var command = new InitializePlayerCommand(playerName, gameId);
        var grainState = new PlayerState { IsInitialized = false };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.GameId': 'GameId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerNameIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = string.Empty; // Invalid player name
        var gameId = Guid.NewGuid();
        var command = new InitializePlayerCommand(playerName, gameId);
        var grainState = new PlayerState { IsInitialized = false };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.PlayerName': 'PlayerName is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerNameExceedsMaxLength()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = new string('A', 51); // Invalid player name (exceeds 50 characters)
        var gameId = Guid.NewGuid();
        var command = new InitializePlayerCommand(playerName, gameId);
        var grainState = new PlayerState { IsInitialized = false };

        var validator = new InitializePlayerCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.PlayerName': 'PlayerName cannot exceed 50 characters' (Parameter 'command')", exception.Message);
    }

}
