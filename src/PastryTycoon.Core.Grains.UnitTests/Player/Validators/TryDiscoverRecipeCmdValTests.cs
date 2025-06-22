using System;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.Validators;

namespace PastryTycoon.Core.Grains.UnitTests.Player.Validators;

public class TryDiscoverRecipeCmdValTests
{
    private readonly TryDiscoverRecipeCmdVal validator;

    public TryDiscoverRecipeCmdValTests()
    {
        // Initialize the validator
        validator = new TryDiscoverRecipeCmdVal();
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var cmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var ingredientIds = new[] { "test-ingredient-1", "test-ingredient-2" };
        var command = new TryDiscoverRecipeCmd(cmdId, playerId, ingredientIds);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenPlayerIdIsEmpty()
    {
        // Arrange
        var cmdId = Guid.NewGuid();
        var playerId = Guid.Empty;
        var ingredientIds = new[] { "test-ingredient-1", "test-ingredient-2" };
        var command = new TryDiscoverRecipeCmd(cmdId, playerId, ingredientIds);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TryDiscoverRecipeCmd.PlayerId));
    }

    [Fact]
    public void Validate_ShouldReturnFailure_WhenIngredientIdsIsEmpty()
    {
        // Arrange
        var cmdId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var ingredientIds = Array.Empty<string>();
        var command = new TryDiscoverRecipeCmd(cmdId, playerId, ingredientIds);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TryDiscoverRecipeCmd.IngredientIds));
    }
}
