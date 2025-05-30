using System;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

/// <summary>
/// Interface for unlock handlers that check conditions for achievements.
/// </summary>
public interface IUnlockHandler
{
        /// <summary>
        /// Checks if the given player event meets the unlock conditions for an achievement.
        /// </summary>
        /// <param name="playerEvent">The player event that triggered the check.</param>
        /// <param name="state">The current achievements state of the player.</param>
        /// <returns></returns>
        Task<UnlockResult> CheckUnlockConditionAsync(PlayerEvent playerEvent, AchievementsState state);
}
