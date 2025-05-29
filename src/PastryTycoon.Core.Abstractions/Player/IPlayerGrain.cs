using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

public interface IPlayerGrain : IGrainWithGuidKey
{
    public Task DiscoverRecipeAsync(DiscoverRecipeCommand command);
    public Task UnlockAchievementAsync(string achievement, DateTime unlockedAtUtc);
}
