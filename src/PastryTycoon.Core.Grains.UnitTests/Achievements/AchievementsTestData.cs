using System;
using System.Collections;
using System.Linq.Expressions;
using PastryTycoon.Core.Grains.Achievements;
using PastryTycoon.Core.Grains.Achievements.UnlockHandlers;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.UnitTests.Achievements;

public static class AchievementsTestData
{
    /// <summary>
    /// Provides test data for unlocking achievements based on player events and grain state.
    /// </summary>
    /// <remarks>
    ///     <para>Example usage:</para>
    ///     <code>
    ///     [
    ///         new PlayerEvent(...),
    ///         new AchievementsState { ... },
    ///         AchievementConstants.SOME_ACHIEVEMENT_ID,
    ///         (Expression<Func<AchievementsState, bool>>)(state => state.RecipesDiscovered == 1)
    ///     ],
    ///     </code>
    /// </remarks>
    public static IEnumerable<object[]> UnlockAchievementIdForGivenEventAndState =>
    [
        
        [
            new RecipeDiscoveredEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new AchievementsState { RecipesDiscovered = 0, RareIngredientsUsed = new() },
            AchievementConstants.FIRST_RECIPE_DISCOVERED,
            (Expression<Func<AchievementsState, bool>>)(state => state.RecipesDiscovered == 1)
        ]
    ];
}
