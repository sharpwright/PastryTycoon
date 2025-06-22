using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;

namespace PastryTycoon.Core.Grains.UnitTests.Player.CommandHandlers;

public class UnlockAchievementCmdHdlrTsts
{
    private readonly Mock<IValidator<UnlockAchievementCmd>> validatorMock;
    public UnlockAchievementCmdHdlrTsts()
    {
        // Initialize any required services or mocks here
        validatorMock = new Mock<IValidator<UnlockAchievementCmd>>();
    }

    [Fact]
    public async Task Handle_ShouldUnlockAchievement_WhenValidCommand()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "test-achievement";
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var handler = new UnlockAchievementCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = true, PlayerId = playerId };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Event);
        Assert.IsType<PlayerUnlockedAchievementEvent>(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAchievementAlreadyUnlocked()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "test-achievement";
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var handler = new UnlockAchievementCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState
        {
            IsInitialized = true,
            PlayerId = playerId,
            UnlockedAchievements = { [achievementId] = unlockedAtUtc }
        };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Achievement is already unlocked.", result.Errors[0]);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlayerStateInvalid()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "test-achievement";
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var handler = new UnlockAchievementCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = false };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Invalid player state for unlocking achievement.", result.Errors[0]);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlayerIdMismatch()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = "test-achievement";
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCmd(Guid.NewGuid(), achievementId, unlockedAtUtc); // Mismatched PlayerId
        var handler = new UnlockAchievementCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = true, PlayerId = playerId };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Invalid player state for unlocking achievement.", result.Errors[0]);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var achievementId = string.Empty; // Invalid achievement ID
        var unlockedAtUtc = DateTime.UtcNow;
        var command = new UnlockAchievementCmd(playerId, achievementId, unlockedAtUtc);
        var handler = new UnlockAchievementCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = true, PlayerId = playerId };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("AchievementId", "Achievement ID cannot be empty.")
            }));

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Achievement ID cannot be empty.", result.Errors[0]);
    }
}
