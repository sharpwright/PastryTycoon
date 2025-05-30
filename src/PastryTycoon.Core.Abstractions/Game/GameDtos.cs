using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

/// <summary>
/// Data Transfer Object (DTO) for game statistics.
/// </summary>
/// <param name="GameId">The unique identifier of the game.</param>
/// <param name="PlayerId">The unique identifier of the player.</param>
/// <param name="TotalRecipes">The total number of recipes discovered by the player in the game.</param>
/// <param name="StartTimeUtc">The UTC timestamp when the game started.</param>
/// <param name="LastUpdatedUtc">The UTC timestamp when the game statistics were last updated.</param>
[GenerateSerializer]
public record GameStatisticsDto(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] int TotalRecipes,
    [property: Id(3)] DateTime StartTimeUtc,
    [property: Id(4)] DateTime LastUpdatedUtc);
