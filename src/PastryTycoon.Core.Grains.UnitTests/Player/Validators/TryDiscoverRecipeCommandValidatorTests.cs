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
        var ingredientIds = new List<string> { "ingredient-1", "ingredient-2" };
        var command = new TryDiscoverRecipeCommand(playerId, ingredientIds);
        var state = new PlayerState();

        var validator = new TryDiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => validator.ValidateCommandAndThrowsAsync(command, state, playerId));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var playerId = Guid.Empty; // Invalid player ID
        var ingredientIds = new List<string> { "ingredient-1", "ingredient-2" };
        var command = new TryDiscoverRecipeCommand(playerId, ingredientIds);
        var grainState = new PlayerState();

        var validator = new TryDiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.PlayerId': 'PlayerId is required' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenIngredientIdsIsEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var ingredientIds = new List<string>(); // Empty ingredient IDs
        var command = new TryDiscoverRecipeCommand(playerId, ingredientIds);
        var grainState = new PlayerState();

        var validator = new TryDiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.IngredientIds': 'At least one ingredient ID must be provided' (Parameter 'command')", exception.Message);
    }

    [Fact]
    public async Task ValidateCommandOrThrowsAsync_ShouldThrowArgumentException_WhenIngredientIdsINull()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        List<string>? ingredientIds = null; // Null ingredient IDs
        var command = new TryDiscoverRecipeCommand(playerId, ingredientIds!);
        var grainState = new PlayerState();

        var validator = new TryDiscoverRecipeCommandValidator();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await validator.ValidateCommandAndThrowsAsync(command, grainState, playerId));

        Assert.Equal("'Command.IngredientIds': 'At least one ingredient ID must be provided' (Parameter 'command')", exception.Message);
    }
}
