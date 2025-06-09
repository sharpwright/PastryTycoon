using System;
using Moq;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Core.Grains.UnitTests.Player.CommandHandlers;

public class TryDiscoverRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> recipeRepositoryMock;
    private readonly Mock<IGrainValidator<TryDiscoverRecipeCommand, PlayerState, Guid>> grainValidatorMock;
    private readonly Guid playerId = Guid.NewGuid();
    private readonly PlayerState playerState = new()
    {
        PlayerId = Guid.NewGuid(),
        IsInitialized = true,
    };

    public TryDiscoverRecipeCommandHandlerTests()
    {
        // Initialize any required services or mocks here
        recipeRepositoryMock = new Mock<IRecipeRepository>();
        grainValidatorMock = new Mock<IGrainValidator<TryDiscoverRecipeCommand, PlayerState, Guid>>();
    }

    [Fact]
    public async Task Handle_ShouldDiscoverRecipe_WhenConditionsMet()
    {
        // Arrange
        var command = new TryDiscoverRecipeCommand(playerId, ["test-ingredient-1", "test-ingredient-2"]);

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Recipe(
                "test-recipe-1",
                It.IsAny<string>(),
                It.IsAny<List<RecipeIngredient>>()
            ));

        var handler = new TryDiscoverRecipeCommandHandler(
            recipeRepositoryMock.Object,
            grainValidatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PlayerDiscoveredRecipeEvent>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRecipeAlreadyDiscovered()
    {
        // Arrange
        var command = new TryDiscoverRecipeCommand(playerId, ["test-ingredient-1", "test-ingredient-2"]);
        playerState.DiscoveredRecipes = new Dictionary<string, DateTime>
        {
            { "test-recipe-1", DateTime.UtcNow }
        };

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Recipe(
                "test-recipe-1",
                It.IsAny<string>(),
                It.IsAny<List<RecipeIngredient>>()
            ));

        var handler = new TryDiscoverRecipeCommandHandler(
            recipeRepositoryMock.Object,
            grainValidatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRecipeNotFound()
    {
        // Arrange
        var command = new TryDiscoverRecipeCommand(playerId, ["test-ingredient-1", "test-ingredient-2"]);

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync((Recipe)null!);

        var handler = new TryDiscoverRecipeCommandHandler(
            recipeRepositoryMock.Object,
            grainValidatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenValidationFails()
    {
        // Arrange
        var command = new TryDiscoverRecipeCommand(playerId, ["test-ingredient-1", "test-ingredient-2"]);
        
        grainValidatorMock
            .Setup(v => v.ValidateCommandAndThrowsAsync(command, playerState, playerId))
            .ThrowsAsync(new ArgumentException());

        var handler = new TryDiscoverRecipeCommandHandler(
            recipeRepositoryMock.Object,
            grainValidatorMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(command, playerState, playerId));
    }
}
