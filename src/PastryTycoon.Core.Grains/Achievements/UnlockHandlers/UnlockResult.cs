using System;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

public class UnlockResult
{
    public bool IsUnlocked { get; }
    public string? AchievementId { get; }

    private UnlockResult(bool isUnlocked, string? achievementId = null)
    {
        IsUnlocked = isUnlocked;
        AchievementId = achievementId;
    }

    public static UnlockResult Success(string achievementId) => new UnlockResult(true, achievementId);

    public static UnlockResult Failure() => new UnlockResult(false);
}
