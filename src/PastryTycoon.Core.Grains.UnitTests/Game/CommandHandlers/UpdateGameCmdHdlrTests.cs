using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.Game.CommandHandlers;

namespace PastryTycoon.Core.Grains.UnitTests.Game.CommandHandlers;

public class UpdateGameCmdHdlrTests
{
    private readonly Mock<IValidator<UpdateGameCmd>> validatorMock;

    public UpdateGameCmdHdlrTests()
    {
        // Initialize the validator mock
        validatorMock = new Mock<IValidator<UpdateGameCmd>>();
    }

    [Fact]
    public async Task Handle_ShouldUpdateGameState_WhenValidCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var command = new UpdateGameCmd(gameId, DateTime.UtcNow);
        var gameState = new GameState { IsInitialized = true, GameId = gameId.ToString("N") };
        var handler = new UpdateGameCmdHdlr(validatorMock.Object);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, gameState, gameId.ToString("N"));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Event);
        Assert.IsType<GameUpdatedEvent>(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenGameStateNotInitialized()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var command = new UpdateGameCmd(gameId, DateTime.UtcNow);
        var gameState = new GameState { IsInitialized = false, GameId = gameId.ToString("N") };
        var handler = new UpdateGameCmdHdlr(validatorMock.Object);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, gameState, gameId.ToString("N"));

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Game state is not initialized.", result.Errors[0]);
        Assert.Null(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var gameId = Guid.Empty; // Invalid game ID
        var command = new UpdateGameCmd(gameId, DateTime.UtcNow);
        var gameState = new GameState { IsInitialized = true, GameId = gameId.ToString("N") };
        var handler = new UpdateGameCmdHdlr(validatorMock.Object);

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("GameId", "GameId is required.")
        };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        var result = await handler.HandleAsync(command, gameState, gameId.ToString("N"));

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("GameId is required.", result.Errors[0]);
        Assert.Null(result.Event);
    }
}
