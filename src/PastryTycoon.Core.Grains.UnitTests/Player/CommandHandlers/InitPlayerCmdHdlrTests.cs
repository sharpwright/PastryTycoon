using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;

namespace PastryTycoon.Core.Grains.UnitTests.Player.CommandHandlers;

public class InitPlayerCmdHdlrTests
{
    private readonly Mock<IValidator<InitPlayerCmd>> validatorMock;

    public InitPlayerCmdHdlrTests()
    {
        // Initialize any required services or mocks here
        validatorMock = new Mock<IValidator<InitPlayerCmd>>();
    }

    [Fact]
    public async Task Handle_ShouldInitializePlayer_WhenValidCommand()
    {
        // Arrange
        var primaryKey = Guid.NewGuid();
        var command = new InitPlayerCmd("Test Player", Guid.NewGuid());
        var handler = new InitPlayerCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = false };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, primaryKey);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Event);
        Assert.IsType<PlayerInitializedEvent>(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlayerAlreadyInitialized()
    {
        // Arrange
        var primaryKey = Guid.NewGuid();
        var command = new InitPlayerCmd("Test Player", Guid.NewGuid());
        var handler = new InitPlayerCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = true };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, playerState, primaryKey);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Event);
        Assert.NotNull(result.Errors);
        Assert.Contains("Player is already initialized.", result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var primaryKey = Guid.NewGuid();
        var command = new InitPlayerCmd("", Guid.NewGuid()); // Invalid player name
        var handler = new InitPlayerCmdHdlr(validatorMock.Object);
        var playerState = new PlayerState { IsInitialized = false };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure(nameof(command.PlayerName), "Player name cannot be empty.")
        });

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await handler.HandleAsync(command, playerState, primaryKey);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Event);
        Assert.NotNull(result.Errors);
        Assert.Contains("Player name cannot be empty.", result.Errors);
    }
}
