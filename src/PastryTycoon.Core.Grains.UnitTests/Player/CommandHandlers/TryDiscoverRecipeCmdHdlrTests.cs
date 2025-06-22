using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using PastryTycoon.Core.Abstractions.Player;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Grains.Player;
using PastryTycoon.Core.Grains.Player.CommandHandlers;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Core.Grains.UnitTests.Player.CommandHandlers;

public class TryDiscoverRecipeCmdHdlrTests
{
    private readonly Mock<IRecipeRepository> recipeRepositoryMock;
    private readonly Mock<IValidator<TryDiscoverRecipeCmd>> validatorMock;
    private readonly Guid playerId = Guid.NewGuid();
    private readonly PlayerState playerState = new();

    public TryDiscoverRecipeCmdHdlrTests()
    {
        // Initialize any required services or mocks here
        recipeRepositoryMock = new Mock<IRecipeRepository>();
        validatorMock = new Mock<IValidator<TryDiscoverRecipeCmd>>();
        playerState.PlayerId = playerId;
        playerState.IsInitialized = true;
    }

    [Fact]
    public async Task Handle_ShouldDiscoverRecipe_WhenConditionsMet()
    {
        // Arrange
        var command = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, ["test-ingredient-1", "test-ingredient-2"]);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Recipe(
                "test-recipe-1",
                It.IsAny<string>(),
                It.IsAny<List<RecipeIngredient>>()
            ));            

        var handler = new TryDiscoverRecipeCmdHdlr(
            recipeRepositoryMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.IsType<PlayerDiscoveredRecipeEvent>(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecipeAlreadyDiscovered()
    {
        // Arrange
        var command = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, ["test-ingredient-1", "test-ingredient-2"]);
        playerState.DiscoveredRecipes = new Dictionary<string, DateTime>
        {
            { "test-recipe-1", DateTime.UtcNow }
        };

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Recipe(
                "test-recipe-1",
                It.IsAny<string>(),
                It.IsAny<List<RecipeIngredient>>()
            ));

        var handler = new TryDiscoverRecipeCmdHdlr(
            recipeRepositoryMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRecipeNotFound()
    {
        // Arrange
        var command = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, ["test-ingredient-1", "test-ingredient-2"]);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        recipeRepositoryMock
            .Setup(repo => repo.GetRecipeByIngredientIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync((Recipe)null!);

        var handler = new TryDiscoverRecipeCmdHdlr(
            recipeRepositoryMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Event);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenValidationFails()
    {
        // Arrange
        var command = new TryDiscoverRecipeCmd(Guid.NewGuid(), playerId, []);

        validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult
            {
                Errors = [new ValidationFailure("Ingredients", "Ingredients cannot be empty.")]
            });

        var handler = new TryDiscoverRecipeCmdHdlr(
            recipeRepositoryMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command, playerState, playerId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
    }
}
