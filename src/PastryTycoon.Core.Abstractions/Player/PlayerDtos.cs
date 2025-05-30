using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

[GenerateSerializer]
public record PlayerStatisticsDto
{
    [Id(0)] public Guid PlayerId { get; init; }
    [Id(1)] public string? PlayerName { get; init; }
    [Id(3)] public int TotalAchievementsUnlocked { get; init; }
    [Id(5)] public int TotalRecipesCrafted { get; init; }
    [Id(6)] public int TotalRecipesDiscovered { get; init; }    
    [Id(7)] public DateTime LastActivityAtUtc { get; init; }
    [Id(8)] public DateTime CreatedAtUtc { get; init; }
}