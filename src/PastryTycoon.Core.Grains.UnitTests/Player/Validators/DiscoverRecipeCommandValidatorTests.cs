using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Player.Validators;

public class DiscoverRecipeCommandValidatorTests
{
    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldReturnTrue_WhenCommandIsValid()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow);
        var state = new PlayerState();

        var validator = new DiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, state, playerId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.Empty; // Invalid player ID
        var recipeId = Guid.NewGuid();
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow);
        var grainState = new PlayerState();

        var validator = new DiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.PlayerId': 'PlayerId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenRecipeIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.Empty; // Invalid recipe ID
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow);
        var grainState = new PlayerState();

        var validator = new DiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.RecipeId': 'RecipeId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenRecipeIdInState()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow);
        var grainState = new PlayerState()
        {
            PlayerId = playerId,
            DiscoveredRecipeIds = new Dictionary<Guid, DateTime> { { recipeId, DateTime.UtcNow } }
        };

        var validator = new DiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.RecipeId': 'RecipeId must not already be discovered by the player' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenDiscoveryTimeUtcIsInTheFuture()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var command = new DiscoverRecipeCommand(playerId, recipeId, DateTime.UtcNow.AddMinutes(10)); // Future time
        var grainState = new PlayerState();

        var validator = new DiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.DiscoveryTimeUtc': 'DiscoveryTimeUtc must be in the past or present' (Parameter 'command')", exception.Message);
    }
}
