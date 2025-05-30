using System;

namespace PastryTycoon.Core.Grains.Player;

[GenerateSerializer]
public record PlayerEvent
{
    [property: Id(0)] public Guid PlayerId { get; init; } = Guid.Empty;
};

[GenerateSerializer]
public record PlayerInitializedEvent : PlayerEvent
{
    [property: Id(1)] public string PlayerName { get; init; } = string.Empty;
    [property: Id(2)] public Guid GameId { get; init; } = Guid.Empty;
    [property: Id(3)] public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
 };

[GenerateSerializer]
public record PlayerDiscoveredRecipeEvent : PlayerEvent
{
    [property: Id(1)] public Guid RecipeId { get; init; } = Guid.Empty;
    [property: Id(2)] public DateTime DiscoveryTimeUtc { get; init; } = DateTime.UtcNow;
};

[GenerateSerializer]
public record PlayerUnlockedAchievementEvent : PlayerEvent
{
    [property: Id(1)] public string AchievementId { get; init; } = string.Empty;
    [property: Id(2)] public DateTime UnlockedAtUtc { get; init; } = DateTime.UtcNow;
};
