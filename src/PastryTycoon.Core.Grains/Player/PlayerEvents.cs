using System;

namespace PastryTycoon.Core.Grains.Player;

[GenerateSerializer]
public record PlayerEvent(
    [property: Id(0)] Guid PlayerId
);

[GenerateSerializer]
public record PlayerInitializedEvent (
    Guid PlayerId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] Guid GameId,
    [property: Id(3)] DateTime CreatedAtUtc
) : PlayerEvent(PlayerId);

[GenerateSerializer]
public record PlayerDiscoveredRecipeEvent(
    Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
) : PlayerEvent(PlayerId);

[GenerateSerializer]
public record PlayerUnlockedAchievementEvent
(
    Guid PlayerId,
    [property: Id(1)] string AchievementId,
    [property: Id(2)] DateTime UnlockedAtUtc
) : PlayerEvent(PlayerId);