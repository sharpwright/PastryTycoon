using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Game;
using PastryTycoon.Core.Grains.Game.CommandHandlers;

namespace PastryTycoon.Core.Grains.UnitTests.Game.CommandHandlers;

public class InitGameStateCmdHdlrTests
{
    private readonly Mock<IValidator<InitGameStateCmd>> validatorMock;

    public InitGameStateCmdHdlrTests()
    {
        // Initialize the validator mock
        validatorMock = new Mock<IValidator<InitGameStateCmd>>();
    }

    [Fact]
    public async Task Handle_ShouldInitializeGameState_WhenValidCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<string>() { "recipe1", "recipe2" };
        var startTimeUtc = DateTime.UtcNow;
        var command = new InitGameStateCmd(gameId, playerId, recipeIds, startTimeUtc);
        var gameState = new GameState { IsInitialized = false, GameId = gameId.ToString("N") };
        var handler = new InitGameStateCmdHdlr(validatorMock.Object);

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
        Assert.IsType<GameStateInitializedEvent>(result.Event);
    }

[Fact]
    public async Task Handle_ShouldReturnFailure_WhenGameStateAlreadyInitialized()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<string>() { "recipe1", "recipe2" };
        var startTimeUtc = DateTime.UtcNow;
        var command = new InitGameStateCmd(gameId, playerId, recipeIds, startTimeUtc);
        var gameState = new GameState { IsInitialized = true, GameId = gameId.ToString("N") };
        var handler = new InitGameStateCmdHdlr(validatorMock.Object);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await handler.HandleAsync(command, gameState, gameId.ToString("N"));

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Game state is already initialized.", result.Errors[0]);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCommandIsInvalid()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var recipeIds = new List<string>() { "recipe1", "recipe2" };
        var startTimeUtc = DateTime.UtcNow;
        var command = new InitGameStateCmd(gameId, playerId, recipeIds, startTimeUtc);
        var gameState = new GameState { IsInitialized = false, GameId = gameId.ToString("N") };
        var handler = new InitGameStateCmdHdlr(validatorMock.Object);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure(nameof(InitGameStateCmd.GameId), "Game ID cannot be empty.")
            }));

        // Act
        var result = await handler.HandleAsync(command, gameState, gameId.ToString("N"));

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Game ID cannot be empty.", result.Errors[0]);
    }

    
}
