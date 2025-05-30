using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

/// <summary>
/// Data Transfer Object (DTO) for Player Statistics.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player.</param>
/// <param name="PlayerName">The name of the player.</param>
/// <param name="TotalAchievementsUnlocked">The total number of achievements unlocked by the player.</param>
/// <param name="TotalRecipesCrafted">The total number of recipes crafted by the player.</param>
/// <param name="TotalRecipesDiscovered">The total number of recipes discovered by the player.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the player was created.</param>
/// <param name="LastActivityAtUtc">The UTC timestamp of the player's last activity.</param>
[GenerateSerializer]
public record PlayerStatisticsDto(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string? PlayerName,
    [property: Id(3)] int TotalAchievementsUnlocked,
    [property: Id(5)] int TotalRecipesCrafted,
    [property: Id(6)] int TotalRecipesDiscovered,
    [property: Id(7)] DateTime CreatedAtUtc,
    [property: Id(8)] DateTime LastActivityAtUtc);