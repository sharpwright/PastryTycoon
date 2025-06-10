using System;
using Orleans;
using PastryTycoon.Core.Abstractions.Common;

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
    public Task<CommandResult> InitializeAsync(InitPlayerCmd command);

    /// <summary>
    /// Discovers a recipe for the player based on the provided command.
    /// </summary>
    /// <param name="command">Command containing the recipe discovery details.</param>
    /// <returns></returns>
    public Task<CommandResult> TryDiscoverRecipeFromIngredientsAsync(TryDiscoverRecipeCmd command);

    /// <summary>
    /// Unlocks an achievement for the player based on the provided command.
    /// </summary>
    /// <param name="command">Command containing the achievement unlock details.</param>
    /// <returns></returns>
    public Task<CommandResult> UnlockAchievementAsync(UnlockAchievementCmd command);

    /// <summary>
    /// Retrieves the player statistics asynchronously.
    /// </summary>
    /// <returns></returns>
    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync();
}
