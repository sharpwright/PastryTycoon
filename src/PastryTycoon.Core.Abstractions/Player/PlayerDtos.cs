using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

[GenerateSerializer]
public record PlayerStatisticsDto(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string? PlayerName,
    [property: Id(3)] int TotalAchievementsUnlocked,
    [property: Id(5)] int TotalRecipesCrafted,
    [property: Id(6)] int TotalRecipesDiscovered,
    [property: Id(7)] DateTime CreatedAtUtc,
    [property: Id(8)] DateTime LastActivityAtUtc);