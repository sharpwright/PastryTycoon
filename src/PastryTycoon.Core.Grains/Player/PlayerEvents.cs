using System;

namespace PastryTycoon.Core.Grains.Player;

/// <summary>
/// Base class for all player-related events.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
[GenerateSerializer]
public record PlayerEvent(
    [property: Id(0)] Guid PlayerId
);

/// <summary>
/// Event that represents a player being initialized in the game.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="PlayerName">The name of the player.</param>
/// <param name="GameId">The unique identifier for the game the player is associated with.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the player was created.</param>
[GenerateSerializer]
public record PlayerInitializedEvent (
    Guid PlayerId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] Guid GameId,
    [property: Id(3)] DateTime CreatedAtUtc
) : PlayerEvent(PlayerId);

/// <summary>
/// Event that represents a player discovering a recipe.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="RecipeId">The unique identifier for the recipe that was discovered.</param>
/// <param name="DiscoveryTimeUtc">Represents the UTC timestamp when the recipe was discovered.</param>
[GenerateSerializer]
public record PlayerDiscoveredRecipeEvent(
    Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
) : PlayerEvent(PlayerId);

/// <summary>
/// Event that represents a player unlocking an achievement.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="AchievementId">The unique identifier for the achievement that was unlocked.</param>
/// <param name="UnlockedAtUtc">Represents the UTC timestamp when the achievement was unlocked.</param>
[GenerateSerializer]
public record PlayerUnlockedAchievementEvent
(
    Guid PlayerId,
    [property: Id(1)] string AchievementId,
    [property: Id(2)] DateTime UnlockedAtUtc
) : PlayerEvent(PlayerId);