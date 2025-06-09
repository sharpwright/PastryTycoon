using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using PastryTycoon.Data.Ingredients;
using PastryTycoon.Data.UnitTests.Ingredients;
using Moq;
using Microsoft.Extensions.Logging;

namespace PastryTycoon.Data.UnitTests.Ingredients
{
    public class IngredientRepositoryTests
    {
        private readonly IMock<ILogger<IngredientRepository>> mockLogger;

        public IngredientRepositoryTests()
        {
            // Create a mock ILogger<IngredientRepository>
            this.mockLogger = new Mock<ILogger<IngredientRepository>>();
        }

        [Fact]
        public async Task GetAllIngredients_ShouldReturnIngredients()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);

            // Act
            var ingredients = await ingredientRepository.GetAllIngredientsAsync();

            // Assert
            Assert.NotNull(ingredients);
            Assert.True(ingredients.Count > 1, "Expected more than one ingredient in the list.");
        }

        [Fact]
        public async Task GetIngredientById_ShouldReturnIngredient()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            var testIngredientId = "pastry-flour"; // Assuming this ID exists in the mock data

            // Act
            var ingredient = await ingredientRepository.GetIngredientByIdAsync(testIngredientId);

            // Assert
            Assert.NotNull(ingredient);
            Assert.Equal(testIngredientId, ingredient.Id);
        }

        [Fact]
        public async Task GetIngredientById_ShouldReturnNull_WhenIngredientDoesNotExist()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            var nonExistentIngredientId = "non-existent-ingredient";

            // Act
            var ingredient = await ingredientRepository.GetIngredientByIdAsync(nonExistentIngredientId);

            // Assert
            Assert.Null(ingredient);
        }

        [Fact]
        public async Task GetIngredientById_ShouldThrowException_WhenIdIsNull()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);

            // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            await Assert.ThrowsAsync<ArgumentException>(() => ingredientRepository.GetIngredientByIdAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public async Task GetIngredientById_ShouldThrowException_WhenIdIsEmpty()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => ingredientRepository.GetIngredientByIdAsync(string.Empty));
        }

        [Fact]
        public async Task GetIngredientsByIds_ShouldReturnIngredients()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            var testIngredientIds = new string[] { "pastry-flour", "granulated-sugar" };

            // Act
            var ingredients = await ingredientRepository.GetIngredientsByIdsAsync(testIngredientIds);

            // Assert
            Assert.NotNull(ingredients);
            Assert.Equal(2, ingredients.Count);
        }

        [Fact]
        public async Task GetIngredientsByIds_ShouldThrowException_WhenNoIdsProvided()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            var emptyIngredientIds = new string[] { };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => ingredientRepository.GetIngredientsByIdsAsync(emptyIngredientIds));
        }

        [Fact]
        public async Task GetIngredientsByIds_ShouldReturnEmptyList_WhenNoMatchingIds()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            var nonExistentIngredientIds = new string[] { "non-existent-ingredient-1", "non-existent-ingredient-2" };

            // Act
            var ingredients = await ingredientRepository.GetIngredientsByIdsAsync(nonExistentIngredientIds);

            // Assert
            Assert.NotNull(ingredients);
            Assert.Empty(ingredients);
        }

        [Fact]
        public async Task GetIngredientsByIds_ShouldThrowException_WhenIdsAreNull()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => ingredientRepository.GetIngredientsByIdsAsync(null!));
        }
    }
}
