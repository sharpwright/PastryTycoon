using System;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

/// <summary>
/// Represents the result of an achievement unlock attempt.
/// </summary>
public class UnlockResult
{
    /// <summary>
    /// Indicates whether the achievement was successfully unlocked.
    /// </summary>
    public bool IsUnlocked { get; }

    /// <summary>
    /// The identifier of the achievement that was unlocked, if applicable.
    /// </summary>
    public string? AchievementId { get; }

    private UnlockResult(bool isUnlocked, string? achievementId = null)
    {
        IsUnlocked = isUnlocked;
        AchievementId = achievementId;
    }

    /// <summary>
    /// Creates a successful unlock result for the specified achievement.
    /// </summary>
    /// <param name="achievementId">The identifier of the achievement that was unlocked.</param>
    /// <returns></returns>
    public static UnlockResult Success(string achievementId) => new UnlockResult(true, achievementId);

    /// <summary>
    /// Creates a failure result indicating that the achievement was not unlocked.
    /// </summary>
    /// <returns></returns>
    public static UnlockResult Failure() => new UnlockResult(false);
}
