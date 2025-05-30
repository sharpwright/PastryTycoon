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
}
