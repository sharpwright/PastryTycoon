using System;
using Microsoft.Extensions.Logging;
using Moq;
using PastryTycoon.Data.Recipes;

namespace PastryTycoon.Data.UnitTests.Recipes;

public class RecipeRepositoryTests
{
    private readonly IMock<ILogger<RecipeRepository>> mockLogger;
    public RecipeRepositoryTests()
    {
        this.mockLogger = new Mock<ILogger<RecipeRepository>>();
    }

    [Fact]
    public async Task GetAllRecipes_ShouldReturnRecipes()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);

        // Act
        var recipes = await recipeRepository.GetAllRecipesAsync();

        // Assert
        Assert.NotNull(recipes);
        Assert.True(recipes.Count > 1, "Expected more than one ingredient in the list.");
    }

    [Fact]
    public async Task GetRecipeById_ShouldReturnRecipe()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);
        string testRecipeId = "chocolate-ganache-recipe";

        // Act
        var recipe = await recipeRepository.GetRecipeByIdAsync(testRecipeId);

        // Assert
        Assert.NotNull(recipe);
        Assert.Equal(testRecipeId, recipe.Id);
    }

    [Fact]
    public async Task GetRecipeById_ShouldReturnNull_WhenRecipeDoesNotExist()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);
        string nonExistentRecipeId = "non-existent-recipe";

        // Act
        var recipe = await recipeRepository.GetRecipeByIdAsync(nonExistentRecipeId);

        // Assert
        Assert.Null(recipe);
    }

    [Fact]
    public async Task GetRecipeById_ShouldThrowException_WhenRecipeIdIsNull()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => recipeRepository.GetRecipeByIdAsync(null!));
    }

    [Fact]
    public async Task GetRecipeById_ShouldThrowException_WhenRecipeIdIsEmpty()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => recipeRepository.GetRecipeByIdAsync(string.Empty));
    }

    [Fact]
    public async Task GetRecipeByIngredientIds_ShouldReturnRecipe()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);
        var ingredientIds = new List<string> { "dark-chocolate", "heavy-cream" };

        // Act
        var recipe = await recipeRepository.GetRecipeByIngredientIdsAsync(ingredientIds);

        // Assert
        Assert.NotNull(recipe);
        Assert.Equal("chocolate-ganache-recipe", recipe.Id);
    }

    [Fact]
    public async Task GetRecipeByIngredientIds_ShouldReturnNull_WhenNoRecipeMatches()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);
        var ingredientIds = new List<string> { "non-existent-ingredient" };

        // Act
        var recipe = await recipeRepository.GetRecipeByIngredientIdsAsync(ingredientIds);

        // Assert
        Assert.Null(recipe);
    }

    [Fact]
    public async Task GetRecipeByIngredientIds_ShouldThrowException_WhenIngredientIdsIsNull()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => recipeRepository.GetRecipeByIngredientIdsAsync(null!));
    }

    [Fact]
    public async Task GetRecipeByIngredientIds_ShouldThrowException_WhenIngredientIdsIsEmpty()
    {
        // Arrange
        RecipeRepository recipeRepository = new RecipeRepository(mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => recipeRepository.GetRecipeByIngredientIdsAsync(new List<string>()));
    }
}
