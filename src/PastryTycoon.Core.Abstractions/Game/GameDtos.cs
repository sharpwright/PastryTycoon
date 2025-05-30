using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

[GenerateSerializer]
public record GameStatisticsDto(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] int TotalRecipes,
    [property: Id(3)] DateTime StartTimeUtc,
    [property: Id(4)] DateTime LastUpdatedUtc);
