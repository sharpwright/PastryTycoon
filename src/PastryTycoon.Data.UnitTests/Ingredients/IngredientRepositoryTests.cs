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
        public async Task GetAllIngredients_ReturnsListOfIngredients()
        {
            // Arrange
            var ingredientRepository = new IngredientRepository(mockLogger.Object);
            
            // Act
            var ingredients = await ingredientRepository.GetAllIngredientsAsync();

            // Assert
            Assert.NotNull(ingredients);
            Assert.True(ingredients.Count > 1, "Expected more than one ingredient in the list.");
        }
    }
}
