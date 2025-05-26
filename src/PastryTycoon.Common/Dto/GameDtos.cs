using System;
using Orleans;

namespace PastryTycoon.Common.Dto;

[GenerateSerializer]
public record GameStatisticsDto
{
    [Id(0)] public Guid GameId { get; init; }
    [Id(1)] public Guid PlayerId { get; init; }
    [Id(2)] public string? GameName { get; init; }
    [Id(3)] public int TotalRecipes { get; init; }
    [Id(4)] public DateTime StartTimeUtc { get; init; }
    [Id(5)] public DateTime LastUpdatedUtc { get; init; }
}
