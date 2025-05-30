using System;
using System.Threading.Tasks;
using PastryTycoon.Core.Grains.Achievements;
using PastryTycoon.Core.Grains.Achievements.UnlockHandlers;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.UnitTests.Achievements.UnlockHandlers;

public class FirstRecipeDiscoveredUnlockHandlerTests
{
    private readonly FirstRecipeDiscoveredUnlockHandler handler = new();

    public FirstRecipeDiscoveredUnlockHandlerTests()
    {
        // Initialize any required resources or mocks here
    }

    [Fact]
    public async Task FirstRecipeDiscoveredUnlockHandler_ShouldUnlockAchievement_WhenFirstRecipeDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var state = new AchievementsState()
        {
            RecipesDiscovered = 1 // Start with no recipes discovered
        };
        var evt = new PlayerDiscoveredRecipeEvent { PlayerId = playerId, RecipeId = recipeId, DiscoveryTimeUtc = DateTime.UtcNow };

        // Act
        var result = await handler.CheckUnlockConditionAsync(evt, state);

        // Assert
        Assert.True(result.IsUnlocked);
        Assert.Equal(AchievementConstants.FIRST_RECIPE_DISCOVERED, result.AchievementId);
    }

    [Fact]
    public async Task FirstRecipeDiscoveredUnlockHandler_ShouldNotUnlockAchievement_WhenMoreThanOneRecipeDiscovered()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var state = new AchievementsState()
        {
            RecipesDiscovered = 2 // More than one recipe discovered
        };
        var evt = new PlayerDiscoveredRecipeEvent { PlayerId = playerId, RecipeId = recipeId, DiscoveryTimeUtc = DateTime.UtcNow };
        
        // Act
        var result = await handler.CheckUnlockConditionAsync(evt, state);

        // Assert
        Assert.False(result.IsUnlocked);
    }
}
