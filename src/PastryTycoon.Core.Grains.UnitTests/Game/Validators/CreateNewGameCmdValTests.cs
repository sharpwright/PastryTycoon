using System;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators;

public class CreateNewGameCmdValTests
{
    [Fact]
    public async Task Validate_ShouldReturnValidResult_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateNewGameCmd(Guid.NewGuid(), "Test Player", DifficultyLevel.Easy);
        var validator = new CreateNewGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidResult_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var command = new CreateNewGameCmd(Guid.Empty, "Test Player", DifficultyLevel.Easy);
        var validator = new CreateNewGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateNewGameCmd.PlayerId));
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidResult_WhenPlayerNameIsEmpty()
    {
        // Arrange
        var command = new CreateNewGameCmd(Guid.NewGuid(), string.Empty, DifficultyLevel.Easy);
        var validator = new CreateNewGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateNewGameCmd.PlayerName));
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidResult_WhenDifficultyLevelIsInvalid()
    {
        // Arrange
        var command = new CreateNewGameCmd(Guid.NewGuid(), "Test Player", (DifficultyLevel)999);
        var validator = new CreateNewGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateNewGameCmd.DifficultyLevel));
    }
}