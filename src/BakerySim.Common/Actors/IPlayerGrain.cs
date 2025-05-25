using System;
using BakerySim.Common.Commands;
using Orleans;

namespace BakerySim.Common.Actors;

public interface IPlayerGrain : IGrainWithGuidKey
{
    public Task DiscoverRecipe(DiscoverRecipeCommand command);
    public Task UnlockAchievementAsync(string achievement, DateTime unlockedAtUtc);
}
