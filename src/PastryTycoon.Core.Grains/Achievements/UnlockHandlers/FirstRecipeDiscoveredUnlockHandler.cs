using System;
using System.Reflection.Metadata.Ecma335;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

/// <summary>
/// Unlock handler for the achievement of discovering the first recipe.
/// </summary>
public class FirstRecipeDiscoveredUnlockHandler : IUnlockHandler
{
    public Task<UnlockResult> CheckUnlockConditionAsync(PlayerEvent playerEvent, AchievementsState state)
    {
        if (playerEvent is PlayerDiscoveredRecipeEvent)
        {
            if (state.RecipesDiscovered == 1)
            {
                return Task.FromResult(UnlockResult.Success(AchievementConstants.FIRST_RECIPE_DISCOVERED));
            }
        }

        return Task.FromResult(UnlockResult.Failure());
    }
}
