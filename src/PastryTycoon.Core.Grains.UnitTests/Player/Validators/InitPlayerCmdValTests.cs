using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Player.Validators;

public class InitPlayerCmdValTests
{
    private readonly InitPlayerCmdVal validator;

    public InitPlayerCmdValTests()
    {
        // Initialize the validator
        validator = new InitPlayerCmdVal();
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var playerName = "Test Player";
        var gameId = Guid.NewGuid();
        var command = new InitPlayerCmd(playerName, gameId);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenPlayerNameIsEmpty()
    {
        // Arrange
        var playerName = string.Empty;
        var gameId = Guid.NewGuid();
        var command = new InitPlayerCmd(playerName, gameId);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitPlayerCmd.PlayerName));
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenPlayerNameExceedsMaxLength()
    {
        // Arrange
        var playerName = new string('A', 51); // 51 characters long
        var gameId = Guid.NewGuid();
        var command = new InitPlayerCmd(playerName, gameId);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitPlayerCmd.PlayerName));
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenGameIdIsEmpty()
    {
        // Arrange
        var playerName = "Test Player";
        var gameId = Guid.Empty;
        var command = new InitPlayerCmd(playerName, gameId);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(InitPlayerCmd.GameId));
    }    
}
