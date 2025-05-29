using System;
using PastryTycoon.Core.Grains.Player;

namespace PastryTycoon.Core.Grains.Achievements.UnlockHandlers;

public interface IUnlockHandler
{
        Task<UnlockResult> CheckUnlockConditionAsync(PlayerEvent playerEvent, AchievementsState state);
}
