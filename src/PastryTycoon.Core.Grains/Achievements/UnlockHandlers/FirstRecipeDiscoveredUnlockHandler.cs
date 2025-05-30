using System;
using System.Reflection.Metadata.Ecma335;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

public class FirstRecipeDiscoveredUnlockHandler : IUnlockHandler
{
    public Task<UnlockResult> CheckUnlockConditionAsync(PlayerEvent playerEvent, AchievementsState state)
    {
        if (playerEvent is PlayerDiscoveredRecipeEvent evt)
        {
            if (state.RecipesDiscovered == 1) 
            {
                return Task.FromResult(UnlockResult.Success(AchievementConstants.FIRST_RECIPE_DISCOVERED));
            }
        }

        return Task.FromResult(UnlockResult.Failure());
    }
}
