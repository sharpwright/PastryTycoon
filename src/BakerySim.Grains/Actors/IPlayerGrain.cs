using System;
using BakerySim.Grains.Commands;

namespace BakerySim.Grains.Actors;

public interface IPlayerGrain : IGrainWithGuidKey
{
    public Task DiscoverRecipe(DiscoverRecipeCommand command);
    public Task UnlockAchievementAsync(string achievement, DateTime unlockedAtUtc);
}
