using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

[GenerateSerializer]
public record InitializePlayerCommand(
    [property: Id(0)] string PlayerName,
    [property: Id(1)] Guid GameId
);

[GenerateSerializer]
public record DiscoverRecipeCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
);

[GenerateSerializer]
public record UnlockAchievementCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string AchievementId,
    [property: Id(2)] DateTime UnlockedAtUtc
);