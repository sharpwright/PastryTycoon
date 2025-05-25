using System;
using PastryTycoon.Common.Commands;
using Orleans;

namespace PastryTycoon.Common.Actors;

public interface IPlayerGrain : IGrainWithGuidKey
{
    public Task DiscoverRecipeAsync(DiscoverRecipeCommand command);
    public Task UnlockAchievementAsync(string achievement, DateTime unlockedAtUtc);
}
