using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Player.Validators;

public class UnlockAchievementCommandValidatorTests
{
    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldReturnTrue_WhenCommandIsValid()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "ACHIEVEMENT_ID";
        var command = new UnlockAchievementCommand(playerId, achievementId, DateTime.UtcNow);
        var state = new PlayerState();

        var validator = new UnlockAchievementCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, state, playerId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.Empty; // Invalid player ID
        var achievementId = "ACHIEVEMENT_ID";
        var command = new UnlockAchievementCommand(playerId, achievementId, DateTime.UtcNow);
        var grainState = new PlayerState();

        var validator = new UnlockAchievementCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.PlayerId': 'PlayerId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenAchievementIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = String.Empty; // Invalid achievement ID
        var command = new UnlockAchievementCommand(playerId, achievementId, DateTime.UtcNow);
        var grainState = new PlayerState();

        var validator = new UnlockAchievementCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.AchievementId': 'AchievementId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenUnlockedAtUtcIsInFuture()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "ACHIEVEMENT_ID";
        var command = new UnlockAchievementCommand(playerId, achievementId, DateTime.UtcNow.AddMinutes(10)); // Future time
        var grainState = new PlayerState();

        var validator = new UnlockAchievementCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.UnlockedAtUtc': 'UnlockedAtUtc must be in the past or present' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenAchievementAlreadyUnlocked()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "ACHIEVEMENT_ID";
        var command = new UnlockAchievementCommand(playerId, achievementId, DateTime.UtcNow);
        var grainState = new PlayerState
        {
            UnlockedAchievements = { [achievementId] = DateTime.UtcNow }
        };

        var validator = new UnlockAchievementCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.AchievementId': 'AchievementId must not already be unlocked by the player' (Parameter 'command')", exception.Message);
    }
}
