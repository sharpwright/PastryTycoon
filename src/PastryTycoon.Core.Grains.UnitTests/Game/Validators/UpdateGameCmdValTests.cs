using System;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Game.Validators;

public class UpdateGameCmdValTests
{
    [Fact]
    public async Task Validate_ShouldReturnValidResult_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateGameCmd(
            Guid.NewGuid(),
            DateTime.UtcNow);

        var validator = new UpdateGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidResult_WhenGameIdIsEmpty()
    {
        // Arrange
        var command = new UpdateGameCmd(
            Guid.Empty,
            DateTime.UtcNow);

        var validator = new UpdateGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateGameCmd.GameId));
    }

    [Fact]
    public async Task Validate_ShouldReturnInvalidResult_WhenUpdateTimeUtcIsEmpty()
    {
        // Arrange
        var command = new UpdateGameCmd(
            Guid.NewGuid(),
            default);

        var validator = new UpdateGameCmdVal();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateGameCmd.UpdateTimeUtc));
    }
}
