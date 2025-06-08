using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

/// <summary>
/// Interface for the Player grain.
/// </summary>
public interface IPlayerGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Initializes the player state with the provided command.
    /// </summary>
    /// <param name="command">Command containing player initialization details.</param>
    /// <returns></returns>
    public Task InitializeAsync(InitializePlayerCommand command);

    /// <summary>
    /// Discovers a recipe for the player based on the provided command.
    /// </summary>
    /// <param name="command">Command containing the recipe discovery details.</param>
    /// <returns></returns>
    public Task TryDiscoverRecipeFromIngredientsAsync(TryDiscoverRecipeCommand command);

    /// <summary>
    /// Unlocks an achievement for the player based on the provided command.
    /// </summary>
    /// <param name="command">Command containing the achievement unlock details.</param>
    /// <returns></returns>
    public Task UnlockAchievementAsync(UnlockAchievementCommand command);

    /// <summary>
    /// Retrieves the player statistics asynchronously.
    /// </summary>
    /// <returns></returns>
    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync();
}
