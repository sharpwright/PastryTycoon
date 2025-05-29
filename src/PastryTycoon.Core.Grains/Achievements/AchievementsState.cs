using System;

namespace PastryTycoon.Core.Grains.Achievements;

[GenerateSerializer]
public record AchievementsState
{
    [Id(0)] public int RecipesDiscovered { get; set; } = 0;
    [Id(1)] public HashSet<Guid> RareIngredientsUsed { get; init; } = new();
}
