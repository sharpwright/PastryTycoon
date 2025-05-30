using System;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators;

public class CreateNewGameCommandValidatorTests
{
    [Fact]
    public async Task ValidateCommandAndThrowsAsync_ValidCommand_ShouldNotThrow()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var difficultyLevel = DifficultyLevel.Easy;
        var command = new CreateNewGameCommand(playerId, playerName, difficultyLevel);

        var validator = new CreateNewGameCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, new(), Guid.Empty));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.Empty; // Invalid player ID
        var playerName = "Test Player";
        var difficultyLevel = DifficultyLevel.Easy;
        var command = new CreateNewGameCommand(playerId, playerName, difficultyLevel);

        var validator = new CreateNewGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), Guid.Empty));

        Assert.Equal("'Command.PlayerId': 'PlayerId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerNameIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = string.Empty; // Invalid player name
        var difficultyLevel = DifficultyLevel.Easy;
        var command = new CreateNewGameCommand(playerId, playerName, difficultyLevel);

        var validator = new CreateNewGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), Guid.Empty));

        Assert.Equal("'Command.PlayerName': 'PlayerName is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenDifficultyLevelIsInvalid()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var difficultyLevel = (DifficultyLevel)999; // Invalid difficulty level
        var command = new CreateNewGameCommand(playerId, playerName, difficultyLevel);

        var validator = new CreateNewGameCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, new(), Guid.Empty));

        Assert.Equal("'Command.DifficultyLevel': 'DifficultyLevel is invalid' (Parameter 'command')", exception.Message);
    }
}