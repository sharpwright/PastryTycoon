using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

public interface IPlayerGrain : IGrainWithGuidKey
{
    public Task InitializeAsync(InitializePlayerCommand command);
    public Task DiscoverRecipeAsync(DiscoverRecipeCommand command);
    public Task UnlockAchievementAsync(UnlockAchievementCommand command);
    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync();
}
