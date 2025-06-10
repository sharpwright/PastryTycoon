using System;

namespace PastryTycoon.Core.Grains.Achievements;

/// <summary>
/// Represents the state of achievements for a player.
/// </summary>
[Alias("AchievementsState")]
[GenerateSerializer]
public record AchievementsState
{
    [Id(0)] public int RecipesDiscovered { get; set; } = 0;
    [Id(1)] public HashSet<Guid> RareIngredientsUsed { get; init; } = new();
}
