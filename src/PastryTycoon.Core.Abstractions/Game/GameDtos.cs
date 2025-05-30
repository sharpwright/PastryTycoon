using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

[GenerateSerializer]
public record GameStatisticsDto
{
    [Id(0)] public Guid GameId { get; init; }
    [Id(1)] public Guid PlayerId { get; init; }
    [Id(2)] public int TotalRecipes { get; init; }
    [Id(3)] public DateTime StartTimeUtc { get; init; }
    [Id(4)] public DateTime LastUpdatedUtc { get; init; }
}
