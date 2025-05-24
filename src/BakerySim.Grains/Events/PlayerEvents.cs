using System;

namespace BakerySim.Grains.Events;

[GenerateSerializer]
public record PlayerEvent(
    [property: Id(0)] Guid PlayerId
);

[GenerateSerializer]
public record RecipeDiscoveredEvent(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
) : PlayerEvent(
    PlayerId
);

[GenerateSerializer]
public record AchievementUnlockedEvent(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string Achievement,
    [property: Id(2)] DateTime UnlockedAtUtc
) : PlayerEvent(
    PlayerId
);
